import './components/loader/loader.scss';

function show(el: HTMLElement) {
    el.style.display = 'block';
    el.style.visibility = 'visible';
}

function mountApp(e: Event) {

    document.getElementsByClassName('loader-wrapper')[0].remove();

    show(document.getElementById('app'));

    window['EntryPoint'].run(function (vue) {
        vue.$mount('#app');
    });
};

(() => {

    let elapsed: number = 0;
    let handle: number = null;
    let interval: number = 100;

    show(<HTMLElement>document.getElementsByClassName('loader-wrapper')[0]);
    show(<HTMLElement>document.getElementsByClassName('loader-status')[0]);

    var el: HTMLElement = <HTMLElement>document.getElementsByClassName('loader-progress')[0];

    let updateProgress = () => {
        elapsed = elapsed + interval;
        let data = (elapsed * 0.001).toString().split('.')
        let progress = data[0].toString() + '.' + (data.length > 1 ? data[1].substring(0, 1) : 0);
        el.innerText = progress + ' ms';
    }

    handle = window.setInterval(updateProgress, interval);

    document.addEventListener('DOMContentLoaded', (e) => {
        updateProgress();
        window.clearInterval(handle);
        mountApp(e);
    });

})();


