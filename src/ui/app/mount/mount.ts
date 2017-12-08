import { filter, map } from 'lodash';
import { default as networkLoader, INetworkResource } from './network-loader';
import './mount.scss';

function show(el: HTMLElement) {
    el.style.display = 'block';
    el.style.visibility = 'visible';
}

const loadAssets = (assets: string[]) => {

    let resources: INetworkResource[] = map(assets, (asset: string) => {

        var resource: INetworkResource = {
            completed: false,
            el: null,
            type: asset.match(/[\.\/]css/) ? 'css' : 'js',
            uri: asset
        }

        return resource;
    });

    let elapsed: number = 0;
    let handle: number = null;
    let interval: number = 100;

    show(<HTMLElement>document.getElementsByClassName('loader-status')[0]);

    var el: HTMLElement = <HTMLElement>document.getElementsByClassName('loader-progress')[0];
    var elMessage: HTMLElement = <HTMLElement>document.getElementsByClassName('loader-message')[0];

    let updateProgress = () => {
        elapsed = elapsed + interval;
        let data = (elapsed * 0.001).toString().split('.')
        let progress = data[0].toString() + '.' + (data.length > 1 ? data[1].substring(0, 1) : 0);
        el.innerText = progress + ' ms';
    }

    let updateMessage = (items: INetworkResource[]) => {
        let completed = filter(items, (o: INetworkResource) => o.completed).length;
        let total = items.length;
        elMessage.innerText = 'Downloading ' + completed + ' / ' + total + ' network resources';
    };

    let update = () => {
        updateProgress();
        updateMessage(resources);
    };

    handle = window.setInterval(update, interval);

    let onComplete = () => {
        update();
        window.clearInterval(handle);

        document.getElementsByClassName('loader-wrapper')[0].remove();
        show(document.getElementById('app'));

        window['bootstrap'].loadApp(function(vue){ vue.$mount('#app');});
    };

    networkLoader.load(resources, { onComplete: onComplete });
};

(() => {
    window['bootstrap'] = { loadAssets: loadAssets, loadApp: {} };
})();(() => {
    window['bootstrap'] = { loadAssets: loadAssets, loadApp: {} };
})();