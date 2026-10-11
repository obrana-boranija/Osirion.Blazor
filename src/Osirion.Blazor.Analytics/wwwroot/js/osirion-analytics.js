/*
 * Osirion.Blazor.Analytics: reports page views for Blazor enhanced navigation.
 *
 * The tracker scripts of Matomo and Yandex Metrica only count the page that was loaded with a full
 * request. After an enhanced navigation the document is patched in place, so this module listens for
 * Blazor's "enhancedload" event and reports the new location. Google Analytics 4 is not handled here:
 * Enhanced Measurement ("Page changes based on browser history events") already reports those views.
 *
 * Each tracker emits <script src="..." data-osirion-analytics="matomo|yandex" [data-counter]>. The
 * configuration is read from those tags when the event fires, so no inline script is needed.
 */
(function () {
    'use strict';

    if (window.__osirionAnalyticsNavigation) {
        return;
    }
    window.__osirionAnalyticsNavigation = true;

    var previousUrl = window.location.href;

    function reportPageView() {
        var url = window.location.href;
        var referrer = previousUrl;
        previousUrl = url;

        var tags = document.querySelectorAll('script[data-osirion-analytics]');
        for (var i = 0; i < tags.length; i++) {
            var provider = tags[i].getAttribute('data-osirion-analytics');

            if (provider === 'matomo' && window._paq) {
                window._paq.push(['setReferrerUrl', referrer]);
                window._paq.push(['setCustomUrl', url]);
                window._paq.push(['setDocumentTitle', document.title]);
                window._paq.push(['trackPageView']);
            } else if (provider === 'yandex' && typeof window.ym === 'function') {
                var counter = parseInt(tags[i].getAttribute('data-counter'), 10);
                if (!isNaN(counter)) {
                    window.ym(counter, 'hit', url, { title: document.title, referer: referrer });
                }
            }
        }
    }

    function register() {
        if (window.Blazor && typeof window.Blazor.addEventListener === 'function') {
            window.Blazor.addEventListener('enhancedload', reportPageView);
            return true;
        }
        return false;
    }

    // blazor.web.js normally runs before this deferred script; retry once the page finished loading
    // for hosts that start Blazor later.
    if (!register()) {
        window.addEventListener('load', register);
    }
})();
