// Theme Toggle functionality for Osirion.Blazor
(function() {
    'use strict';

    let config = {
        defaultTheme: 'light',
        framework: 'Bootstrap',
        isConfigured: false
    };

    window.setThemeToggleConfig = function(defaultTheme, framework) {
        config.defaultTheme = defaultTheme || 'light';
        config.framework = framework || 'Bootstrap';
        config.isConfigured = true;
        
        if (initialized) {
            updateButtonFromClientState();
        }
    };

    window.toggleThemeWithTransition = function () {
        const html = document.documentElement;
        const framework = html.getAttribute('data-osirion-framework') || config.framework;
        const scrollPosition = html.scrollTop || document.body.scrollTop || 0;
        const currentTheme = html.getAttribute('data-osirion-theme') || config.defaultTheme;
        const newTheme = currentTheme === 'light' ? 'dark' : 'light';

        window.updateThemeToggleButton(newTheme);

        const overlay = document.createElement('div');
        overlay.className = 'theme-transition-overlay';
        overlay.style.cssText = `position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: ${currentTheme === 'light' ? '#000' : '#fff'}; opacity: 0; z-index: 9999; pointer-events: none; transition: opacity 0.3s ease;`;
        document.body.appendChild(overlay);

        requestAnimationFrame(() => { overlay.style.opacity = '0.8'; });

        setTimeout(() => {
            html.setAttribute('data-osirion-theme', newTheme);

            html.removeAttribute('data-bs-theme');
            html.classList.remove('mud-theme-dark', 'fluent-dark-theme', 'fluent-light-theme', 'rz-dark-theme');

            if (framework === 'Bootstrap') {
                html.setAttribute('data-bs-theme', newTheme);
            } else if (framework === 'MudBlazor') {
                html.classList.toggle('mud-theme-dark', newTheme === 'dark');
            } else if (framework === 'FluentUI') {
                html.classList.toggle('fluent-dark-theme', newTheme === 'dark');
                html.classList.toggle('fluent-light-theme', newTheme === 'light');
            } else if (framework === 'Radzen') {
                html.classList.toggle('rz-dark-theme', newTheme === 'dark');
            }

            document.cookie = `osirion-preferred-theme=${newTheme}; path=/; max-age=${365 * 24 * 60 * 60}; SameSite=Lax`;
            document.cookie = `preferred-theme=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT`;

            window.dispatchEvent(new CustomEvent('osirion-theme-update'));

            const originalScroll = scrollPosition;

            setTimeout(() => {
                overlay.style.opacity = '0';
                setTimeout(() => {
                    document.body.removeChild(overlay);
                    window.scrollTo(0, originalScroll);
                    history.replaceState(null, '', window.location.pathname + window.location.search + (window.location.hash || ''));
                }, 300);
            }, 100);
        }, 150);
    };

    window.updateThemeToggleButton = function (overrideTheme) {
        const html = document.documentElement;
        const currentTheme = overrideTheme || html.getAttribute('data-osirion-theme') || config.defaultTheme;
        const toggleBtn = document.getElementById('theme-toggle');
        const icon = toggleBtn?.querySelector('.theme-icon');
        const text = toggleBtn?.querySelector('.theme-text');
        
        if (icon && text && toggleBtn) {
            if (currentTheme === 'dark') {
                icon.outerHTML = '<svg width="16" height="16" fill="currentColor" class="bi bi-sun theme-icon" viewBox="0 0 16 16" aria-hidden="true"><path d="M8 11a3 3 0 1 1 0-6 3 3 0 0 1 0 6zm0 1a4 4 0 1 0 0-8 4 4 0 0 0 0 8zM8 0a.5.5 0 0 1 .5.5v2a.5.5 0 0 1-1 0v-2A.5.5 0 0 1 8 0zm0 13a.5.5 0 0 1 .5.5v2a.5.5 0 0 1-1 0v-2A.5.5 0 0 1 8 13zm8-5a.5.5 0 0 1-.5.5h-2a.5.5 0 0 1 0-1h2a.5.5 0 0 1 .5.5zM3 8a.5.5 0 0 1-.5.5h-2a.5.5 0 0 1 0-1h2A.5.5 0 0 1 3 8zm10.657-5.657a.5.5 0 0 1 0 .707l-1.414 1.414a.5.5 0 1 1-.707-.707l1.414-1.414a.5.5 0 0 1 .707 0zm-9.193 9.193a.5.5 0 0 1 0 .707L3.05 13.657a.5.5 0 0 1-.707-.707l1.414-1.414a.5.5 0 0 1 .707 0zm9.193 2.121a.5.5 0 0 1-.707 0l-1.414-1.414a.5.5 0 0 1 .707-.707l1.414 1.414a.5.5 0 0 1 0 .707zM4.464 4.465a.5.5 0 0 1-.707 0L2.343 3.05a.5.5 0 1 1 .707-.707l1.414 1.414a.5.5 0 0 1 0 .707z"/></svg>';
                toggleBtn.title = 'Switch to light mode';
                toggleBtn.setAttribute('aria-label', 'Toggle theme. Currently using dark mode. Click to switch to light mode.');
                text.textContent = 'Current theme: dark. Click to toggle.';
            } else {
                icon.outerHTML = '<svg width="16" height="16" fill="currentColor" class="bi bi-moon theme-icon" viewBox="0 0 16 16" aria-hidden="true"><path d="M6 .278a.768.768 0 0 1 .08.858 7.208 7.208 0 0 0-.878 3.46c0 4.021 3.278 7.277 7.318 7.277.527 0 1.04-.055 1.533-.16a.787.787 0 0 1 .81.316.733.733 0 0 1-.031.893A8.349 8.349 0 0 1 8.344 16C3.734 16 0 12.286 0 7.71 0 4.266 2.114 1.312 5.124.06A.752.752 0 0 1 6 .278z"/></svg>';
                toggleBtn.title = 'Switch to dark mode';
                toggleBtn.setAttribute('aria-label', 'Toggle theme. Currently using light mode. Click to switch to dark mode.');
                text.textContent = 'Current theme: light. Click to toggle.';
            }
        }
    };

    let initialized = false;
    
    function getClientTheme() {
        const html = document.documentElement;
        
        const match = document.cookie.match(/(?:^|; )osirion-preferred-theme=([^;]*)/);
        if (match) {
            return decodeURIComponent(match[1]);
        }
        
        let theme = html.getAttribute('data-osirion-theme');
        if (theme && theme !== 'null') {
            return theme;
        }
        
        const bsTheme = html.getAttribute('data-bs-theme');
        if (bsTheme === 'dark' || bsTheme === 'light') {
            return bsTheme;
        }
        
        return config.defaultTheme;
    }

    function updateButtonFromClientState() {
        if (!document.getElementById('theme-toggle')) {
            return;
        }
        
        if (!config.isConfigured) {
            setTimeout(updateButtonFromClientState, 100);
            return;
        }
        
        const clientTheme = getClientTheme();
        window.updateThemeToggleButton(clientTheme);
    }

    function initialize() {
        if (initialized) return;
        
        initialized = true;
        
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', updateButtonFromClientState);
        } else {
            requestAnimationFrame(() => {
                setTimeout(updateButtonFromClientState, 10);
            });
        }

        if (typeof Blazor !== 'undefined' && Blazor.addEventListener) {
            Blazor.addEventListener('enhancedload', function() {
                initialized = false;
                
                setTimeout(() => {
                    initialized = true;
                    updateButtonFromClientState();
                }, 100);
                setTimeout(updateButtonFromClientState, 200);
                setTimeout(updateButtonFromClientState, 400);
            });
        }

        window.addEventListener('osirion-theme-update', function() {
            setTimeout(updateButtonFromClientState, 50);
        });
        
        if (typeof MutationObserver !== 'undefined') {
            const observer = new MutationObserver(function(mutations) {
                mutations.forEach(function(mutation) {
                    if (mutation.type === 'attributes' && 
                        (mutation.attributeName === 'data-osirion-theme' || 
                         mutation.attributeName === 'data-bs-theme')) {
                        setTimeout(updateButtonFromClientState, 100);
                    }
                });
            });
            observer.observe(document.documentElement, { 
                attributes: true, 
                attributeFilter: ['data-osirion-theme', 'data-bs-theme'] 
            });
        }

        window.addEventListener('popstate', function() {
            setTimeout(updateButtonFromClientState, 100);
        });
    }

    initialize();
    window.initializeThemeToggle = initialize;
})();