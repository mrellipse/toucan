import { loadScript, loadStyle } from './script-loader';

export interface INetworkResource {
    completed?: boolean,
    el?: Element,
    uri: string,
    type: 'css' | 'js'
}

interface INetworkLoaderEvents {
    onResourceLoaded?: (value: INetworkResource) => void,
    onComplete?: (values: INetworkResource[]) => void
}

const networkLoader = {
    load: (resources: INetworkResource[], cb: INetworkLoaderEvents) => {
        let count = 0;

        let onLoad = () => {
            if (count == resources.length && cb.onComplete)
                cb.onComplete(resources);
        };

        resources.forEach(resource => {

            if (resource.type == 'css') {
                resource.el = loadStyle(resource.uri, (value) => {
                    resource.completed = true;
                    count++;

                    if (cb.onResourceLoaded)
                        cb.onResourceLoaded(resource);

                    onLoad();
                });
            }

            if (resource.type == 'js') {
                resource.el = loadScript(resource.uri, (value) => {
                    resource.completed = true;
                    count++;

                    if (cb.onResourceLoaded)
                        cb.onResourceLoaded(resource);

                    onLoad();
                });
            }
        });
    }
};

export default networkLoader;