(function (global, factory) {
	typeof exports === 'object' && typeof module !== 'undefined' ? factory(exports) :
		typeof define === 'function' && define.amd ? define(['exports'], factory) :
			(global = typeof globalThis !== 'undefined' ? globalThis : global || self, factory(global.THREE = {}));
})(this, (function (exports) {
	'use strict';

	class LoadingManager {
		constructor(onLoad, onProgress, onError) {
			const scope = this;
			let isLoading = false;
			let itemsLoaded = 0;
			let itemsTotal = 0;
			let urlModifier = undefined;
			const handlers = []; // Refer to #5689 for the reason why we don't set .onStart
			// in the constructor

			this.onStart = undefined;
			this.onLoad = onLoad;
			this.onProgress = onProgress;
			this.onError = onError;

			this.itemStart = function (url) {
				itemsTotal++;

				if (isLoading === false) {
					if (scope.onStart !== undefined) {
						scope.onStart(url, itemsLoaded, itemsTotal);
					}
				}

				isLoading = true;
			};

			this.itemEnd = function (url) {
				itemsLoaded++;

				if (scope.onProgress !== undefined) {
					scope.onProgress(url, itemsLoaded, itemsTotal);
				}

				if (itemsLoaded === itemsTotal) {
					isLoading = false;

					if (scope.onLoad !== undefined) {
						scope.onLoad();
					}
				}
			};

			this.itemError = function (url) {
				if (scope.onError !== undefined) {
					scope.onError(url);
				}
			};

			this.resolveURL = function (url) {
				if (urlModifier) {
					return urlModifier(url);
				}

				return url;
			};

			this.setURLModifier = function (transform) {
				urlModifier = transform;
				return this;
			};

			this.addHandler = function (regex, loader) {
				handlers.push(regex, loader);
				return this;
			};

			this.removeHandler = function (regex) {
				const index = handlers.indexOf(regex);

				if (index !== -1) {
					handlers.splice(index, 2);
				}

				return this;
			};

			this.getHandler = function (file) {
				for (let i = 0, l = handlers.length; i < l; i += 2) {
					const regex = handlers[i];
					const loader = handlers[i + 1];
					if (regex.global) regex.lastIndex = 0; // see #17920

					if (regex.test(file)) {
						return loader;
					}
				}

				return null;
			};
		}

	}

	const DefaultLoadingManager = new LoadingManager();
	class Loader {
		constructor(manager) {
			this.manager = manager !== undefined ? manager : DefaultLoadingManager;
			this.crossOrigin = 'anonymous';
			this.withCredentials = false;
			this.path = '';
			this.resourcePath = '';
			this.requestHeader = {};
		}

		load() { }

		loadAsync(url, onProgress) {
			const scope = this;
			return new Promise(function (resolve, reject) {
				scope.load(url, resolve, onProgress, reject);
			});
		}

		parse() { }

		setCrossOrigin(crossOrigin) {
			this.crossOrigin = crossOrigin;
			return this;
		}

		setWithCredentials(value) {
			this.withCredentials = value;
			return this;
		}

		setPath(path) {
			this.path = path;
			return this;
		}

		setResourcePath(resourcePath) {
			this.resourcePath = resourcePath;
			return this;
		}

		setRequestHeader(requestHeader) {
			this.requestHeader = requestHeader;
			return this;
		}

	}

	class FileLoader extends Loader {
		constructor(manager) {
			super(manager);
		}

		load(url, onLoad, onProgress, onError) {
			if (url === undefined) url = '';
			if (this.path !== undefined) url = this.path + url;
			url = this.manager.resolveURL(url);
			const cached = Cache.get(url);

			if (cached !== undefined) {
				this.manager.itemStart(url);
				setTimeout(() => {
					if (onLoad) onLoad(cached);
					this.manager.itemEnd(url);
				}, 0);
				return cached;
			} // Check if request is duplicate


			if (loading[url] !== undefined) {
				loading[url].push({
					onLoad: onLoad,
					onProgress: onProgress,
					onError: onError
				});
				return;
			} // Initialise array for duplicate requests


			loading[url] = [];
			loading[url].push({
				onLoad: onLoad,
				onProgress: onProgress,
				onError: onError
			}); // create request

			const req = new Request(url, {
				headers: new Headers(this.requestHeader),
				credentials: this.withCredentials ? 'include' : 'same-origin' // An abort controller could be added within a future PR

			}); // start the fetch

			fetch(req).then(response => {
				if (response.status === 200 || response.status === 0) {
					// Some browsers return HTTP Status 0 when using non-http protocol
					// e.g. 'file://' or 'data://'. Handle as success.
					if (response.status === 0) {
						console.warn('THREE.FileLoader: HTTP Status 0 received.');
					}

					if (typeof ReadableStream === 'undefined' || response.body.getReader === undefined) {
						return response;
					}

					const callbacks = loading[url];
					const reader = response.body.getReader();
					const contentLength = response.headers.get('Content-Length');
					const total = contentLength ? parseInt(contentLength) : 0;
					const lengthComputable = total !== 0;
					let loaded = 0; // periodically read data into the new stream tracking while download progress

					const stream = new ReadableStream({
						start(controller) {
							readData();

							function readData() {
								reader.read().then(({
									done,
									value
								}) => {
									if (done) {
										controller.close();
									} else {
										loaded += value.byteLength;
										const event = new ProgressEvent('progress', {
											lengthComputable,
											loaded,
											total
										});

										for (let i = 0, il = callbacks.length; i < il; i++) {
											const callback = callbacks[i];
											if (callback.onProgress) callback.onProgress(event);
										}

										controller.enqueue(value);
										readData();
									}
								});
							}
						}

					});
					return new Response(stream);
				} else {
					throw Error(`fetch for "${response.url}" responded with ${response.status}: ${response.statusText}`);
				}
			}).then(response => {
				switch (this.responseType) {
					case 'arraybuffer':
						return response.arrayBuffer();

					case 'blob':
						return response.blob();

					case 'document':
						return response.text().then(text => {
							const parser = new DOMParser();
							return parser.parseFromString(text, this.mimeType);
						});

					case 'json':
						return response.json();

					default:
						return response.text();
				}
			}).then(data => {
				// Add to cache only on HTTP success, so that we do not cache
				// error response bodies as proper responses to requests.
				Cache.add(url, data);
				const callbacks = loading[url];
				delete loading[url];

				for (let i = 0, il = callbacks.length; i < il; i++) {
					const callback = callbacks[i];
					if (callback.onLoad) callback.onLoad(data);
				}
			}).catch(err => {
				// Abort errors and other errors are handled the same
				const callbacks = loading[url];

				if (callbacks === undefined) {
					// When onLoad was called and url was deleted in `loading`
					this.manager.itemError(url);
					throw err;
				}

				delete loading[url];

				for (let i = 0, il = callbacks.length; i < il; i++) {
					const callback = callbacks[i];
					if (callback.onError) callback.onError(err);
				}

				this.manager.itemError(url);
			}).finally(() => {
				this.manager.itemEnd(url);
			});
			this.manager.itemStart(url);
		}

		setResponseType(value) {
			this.responseType = value;
			return this;
		}

		setMimeType(value) {
			this.mimeType = value;
			return this;
		}

	}
	exports.Loader = Loader;
	exports.FileLoader = FileLoader;
	exports.DefaultLoadingManager = DefaultLoadingManager;
	exports.LoadingManager = LoadingManager;
	Object.defineProperty(exports, '__esModule', { value: true });
}))



        //const geu = new TextDecoder("utf-8").decode(data)//解码成字符串
        //const geu = new TextEncoder("utf-8").encode(data)////字符串编码
        //arr.set(fromArr, [offset]) 从 offset（默认为 0）开始，将 fromArr 中的所有元素复制到 arr。
        //arr.subarray([begin, end]) 创建一个从 begin 到 end（不包括）相同类型的新视图。这类似于 
        //slice 方法（同样也支持），但不复制任何内容 —— 只是创建一个新视图，以对给定片段的数据进行操作。
/*
 var dddgf = fetch('/lib/zxhgk.png', {
            method: 'GET', // *GET, POST, PUT, DELETE, etc.
            mode: 'cors', // no-cors, *cors, same-origin
            cache: 'no-cache', // *default, no-cache, reload, force-cache, only-if-cached
            //credentials: 'same-origin', // include, *same-origin, omit
            credentials: 'include',
            headers: {
                //'Content-Type': 'application/json'
                //'Content-Type': 'text/plain'
                // 'Content-Type': 'application/x-www-form-urlencoded',
            },
            redirect: 'follow', // manual, *follow, error
            referrerPolicy: 'no-referrer', // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
            //body: JSON.stringify(data) // body data type must match "Content-Type" header
        })
            .then(response => {
                if (response.status === 200 || response.status === 0) {

                    // Some browsers return HTTP Status 0 when using non-http protocol
                    // e.g. 'file://' or 'data://'. Handle as success.

                    if (response.status === 0) {
                        console.warn('THREE.FileLoader: HTTP Status 0 received.');
                    }

                    if (typeof ReadableStream === 'undefined' || response.body.getReader === undefined) {
                        return response;
                    }

                    const reader = response.body.getReader();
                    //2：获得总长度（length）
                    const contentLength = response.headers.get('Content-Length');
                    const total = contentLength ? parseInt(contentLength) : 0;
                    const lengthComputable = total !== 0;
                    let loaded = 0;

                    //3：读取数据
                    // periodically read data into the new stream tracking while download progress
                    const stream = new ReadableStream({
                        start(controller) {
                            readData();
                            function readData() {
                                reader.read().then(({ done, value }) => {
                                    if (done) {
                                        controller.close();
                                    }
                                    else {
                                        loaded += value.byteLength;
                                        controller.enqueue(value);
                                        readData();
                                    }
                                });
                            }
                        }
                    });
                    return new Response(stream);
                    //let receivedLength = 0; // 当前接收到了这么多字节
                    //let chunks = []; // 接收到的二进制块的数组（包括 body）
                    //reader.read().then(({ done, value }) => {
                    //    if (done) {
                    //        return;
                    //    }
                    //    chunks.push(value);
                    //    loaded += value.byteLength;
                    //    console.log(`Received ${loaded} of ${contentLength}`)
                    //});
                    //return chunks;

                }
                else {
                    throw Error(`fetch for "${response.url}" responded with ${response.status}: ${response.statusText}`);
                }
            })
            .then(response => {
                //return response.json()
                //return response.text()
                return response.blob();
                console.log(response)
            })
            .then(data => {
                //数据
                console.log(data)
            })
            .catch(error => {

                console.error(error);
            })
            .finally(() => {    });

        let response = await fetch('/lib/zxhgk.png');

        const reader = response.body.getReader();

        // Step 2：获得总长度（length）
        const contentLength = +response.headers.get('Content-Length');

        // Step 3：读取数据
        let receivedLength = 0; // 当前接收到了这么多字节
        let chunks = []; // 接收到的二进制块的数组（包括 body）
        while (true) {
            const { done, value } = await reader.read();

            if (done) {
                break;
            }

            chunks.push(value);
            receivedLength += value.length;

            console.log(`Received ${receivedLength} of ${contentLength}`)
        }

        // Step 4：将块连接到单个 Uint8Array
        let chunksAll = new Uint8Array(receivedLength); // (4.1)
        let position = 0;
        for (let chunk of chunks) {
            chunksAll.set(chunk, position); // (4.2)
            position += chunk.length;
        }

        // Step 5：解码成字符串
        let result = new TextDecoder("utf-8").decode(chunksAll);
 

        var df = await fetchJsonFile("/lib/three-dev/editor/manifest.json");
            async function fetchJsonFile(path) {
            const response = await fetch(path);
            if (!response.ok) {
                throw new Error(response.statusText);
            }
            else {
                return response.json();
            }
        }
        //const req = new Request(url, {
        //    headers: new Headers(this.requestHeader),
        //    credentials: this.withCredentials ? 'include' : 'same-origin',
        //    // An abort controller could be added within a future PR
        //});

        //const reqs = new Request("/WebGL/ThreeFile/FileLoading?strpath=" + url, {
        //    headers: new Headers(this.requestHeader),
        //    credentials: this.withCredentials ? 'include' : 'same-origin',
        //    // An abort controller could be added within a future PR
        //});
 */