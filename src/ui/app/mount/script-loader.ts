// attrib: https://gist.github.com/hagenburger/500716

export function loadScript(src: string, callback: (el: HTMLScriptElement) => void): HTMLScriptElement {
    var el = document.createElement('script'), loaded;
    el.src = src;
    el.async = false;

    if (callback) {
        el.onload = function () {
            if (!loaded)
                callback(el);
            loaded = true;
        };
    }
    return document.getElementsByTagName('head')[0].appendChild(el);
};

export function loadStyle(src: string, callback: (el: HTMLLinkElement) => void): HTMLLinkElement {

    var el = document.createElement('link'), loaded;
    el.setAttribute("rel", "stylesheet")
    el.setAttribute("type", "text/css")
    el.setAttribute("href", src)

    if (callback) {
        el.onload = function () {
            if (!loaded)
                callback(el);
            loaded = true;
        };
    }
    return document.getElementsByTagName('head')[0].appendChild(el);
};
