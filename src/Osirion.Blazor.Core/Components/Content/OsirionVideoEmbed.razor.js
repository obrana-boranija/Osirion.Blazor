// Shows the player in place of the thumbnail link when the link is clicked. Until then nothing is loaded from the
// video host except the thumbnail; without script the link opens the video on its own site.
export default class extends BlazorJSComponents.Component {
    setParameters(refs, embedUrl, title) {
        if (!refs.link || !refs.frame || !embedUrl) return;

        this.setEventListener(refs.link, 'click', event => {
            // Let modified clicks (new tab, new window) open the video page as the link says.
            if (event.button !== 0 || event.ctrlKey || event.metaKey || event.shiftKey || event.altKey) return;
            event.preventDefault();

            const iframe = document.createElement('iframe');
            iframe.className = 'osirion-video-embed-player';
            iframe.src = embedUrl;
            iframe.title = title || 'Video';
            iframe.allow = 'accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share';
            iframe.allowFullscreen = true;
            iframe.referrerPolicy = 'strict-origin-when-cross-origin';

            // The link is hidden rather than removed: both elements belong to Blazor's render tree.
            refs.frame.replaceChildren(iframe);
            refs.link.hidden = true;
            iframe.focus();
        });
    }
}
