export function dispatchEvent(target: HTMLElement, event: string, process?: Function) {

    var e = document.createEvent('HTMLEvents');
    e.initEvent(event, true, true);

    if (process)
        process(e);

    target.dispatchEvent(e);
}

export default dispatchEvent;