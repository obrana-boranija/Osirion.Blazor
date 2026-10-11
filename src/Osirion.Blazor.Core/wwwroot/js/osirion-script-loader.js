// Loads classic third-party scripts in order, optionally with Subresource Integrity.
// Used by OsirionHtmlRenderer: the generic JS component loader imports scripts as modules and cannot enforce an
// integrity hash, so scripts that must be verified are added here as <script integrity crossorigin> elements.
// Loaded files are tracked by address, so repeated renders and enhanced navigation never load a file twice.

const loads = new Map();

function loadScript(descriptor) {
    const existing = loads.get(descriptor.src);
    if (existing) {
        return existing;
    }

    const load = new Promise(resolve => {
        const element = document.createElement('script');
        element.src = descriptor.src;
        element.async = false;
        if (descriptor.integrity) {
            element.integrity = descriptor.integrity;
            element.crossOrigin = 'anonymous';
        }

        element.onload = () => resolve(true);
        element.onerror = () => {
            loads.delete(descriptor.src);
            element.remove();
            resolve(false);
        };
        document.head.appendChild(element);
    });

    loads.set(descriptor.src, load);
    return load;
}

export default class extends BlazorJSComponents.Component {
    setParameters(scripts) {
        void this.loadAll(Array.isArray(scripts) ? scripts : []);
    }

    async loadAll(scripts) {
        // Order matters: Prism core must run before its language components and plugins.
        for (const descriptor of scripts) {
            if (!descriptor?.src) {
                continue;
            }

            const loaded = await loadScript(descriptor);
            if (!loaded && descriptor.required) {
                return;
            }
        }

        window.Prism?.highlightAll?.();
    }
}
