// OsirionStyles runtime script
(function(){
  'use strict';

  const GLOBAL_KEY = '__osirionStyles__';
  // The ids of ThemePalette; 'framework' means no palette attribute.
  const PALETTES = ['framework', 'osirion', 'commit', 'editor', 'studio', 'studio-blue', 'contrast'];

  function ensureState(){
    if(!window[GLOBAL_KEY]){
      window[GLOBAL_KEY] = {
        initialized: false,
        config: { 
          frameworkClass: '', 
          selectedFramework: 'Bootstrap', 
          defaultTheme: 'system', 
          currentTheme: 'system',
          defaultPalette: 'framework'
        },
        lastAppliedTheme: null,
        observer: null,
        configReceived: false
      };
    }
    return window[GLOBAL_KEY];
  }

  function getSystemTheme(){
    if (typeof window !== 'undefined' && window.matchMedia) {
      return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
    }
    return 'light';
  }

  function normalizeTheme(value, defaultValue){
    const theme = (value || '').toLowerCase();
    if (theme === 'dark' || theme === 'light') {
      return theme;
    }
    if (theme === 'system') {
      return getSystemTheme();
    }
    
    // Handle defaultValue
    if (defaultValue === 'system') {
      return getSystemTheme();
    }
    return (defaultValue === 'dark' || defaultValue === 'light') ? defaultValue : getSystemTheme();
  }

  function readCookie(name){
    const match = document.cookie.match(new RegExp('(?:^|; )' + name.replace(/([.$?*|{}()\[\]\\/+^])/g,'\\$1') + '=([^;]*)'));
    return match ? decodeURIComponent(match[1]) : null;
  }

  function clearFrameworkMarkers(html){
    if(!html) return;
    html.removeAttribute('data-bs-theme');
    html.classList.remove('mud-theme-dark', 'fluent-dark-theme', 'fluent-light-theme', 'rz-dark-theme');
  }

  function resolveTheme(state){
    const html = document.documentElement;
    const defaultTheme = state.config.defaultTheme;

    const cookie = readCookie('osirion-preferred-theme');
    if(cookie){
      return normalizeTheme(cookie, defaultTheme);
    }

    const attr = html?.getAttribute('data-osirion-theme');
    if(attr && attr !== 'null'){
      return normalizeTheme(attr, defaultTheme);
    }

    return normalizeTheme(defaultTheme, defaultTheme);
  }

  function resolvePalette(state){
    const chosen = (readCookie('osirion-preferred-palette') || '').toLowerCase();
    if(PALETTES.indexOf(chosen) >= 0){
      return chosen;
    }
    return PALETTES.indexOf(state.config.defaultPalette) >= 0 ? state.config.defaultPalette : 'framework';
  }

  function applyPalette(state){
    const html = document.documentElement;
    if (!html) return;
    const palette = resolvePalette(state);
    if(palette === 'framework'){
      html.removeAttribute('data-osirion-palette');
    } else if(html.getAttribute('data-osirion-palette') !== palette){
      html.setAttribute('data-osirion-palette', palette);
    }
  }

  function applyTheme(state, theme){
    const html = document.documentElement;
    if (!html) return;
    
    const normalizedTheme = normalizeTheme(theme, state.config.defaultTheme);
    state.lastAppliedTheme = normalizedTheme;

    html.setAttribute('data-osirion-theme', normalizedTheme);
    html.setAttribute('data-osirion-framework', state.config.selectedFramework);
    applyPalette(state);

    const frameworkClass = state.config.frameworkClass;
    if(frameworkClass && !html.classList.contains(frameworkClass)){
      html.classList.add(frameworkClass);
    }

    clearFrameworkMarkers(html);
    
    const framework = state.config.selectedFramework;
    if(framework === 'Bootstrap'){
      html.setAttribute('data-bs-theme', normalizedTheme);
    } else if(framework === 'MudBlazor'){
      html.classList.toggle('mud-theme-dark', normalizedTheme === 'dark');
    } else if(framework === 'FluentUI'){
      html.classList.toggle('fluent-dark-theme', normalizedTheme === 'dark');
      html.classList.toggle('fluent-light-theme', normalizedTheme === 'light');
      applyFluentTheme(normalizedTheme, 0);
    } else if(framework === 'Radzen'){
      html.classList.toggle('rz-dark-theme', normalizedTheme === 'dark');
    }
  }

  // Fluent UI Blazor v5 keeps its own theme mode and sets its color tokens from it, so Osirion's mode is handed to
  // Blazor.theme.setThemeMode, which also stores it for Fluent's next start. Fluent registers Blazor.theme when Blazor
  // starts; until then body[data-theme] tells it which mode to start in.
  const fluentRetryDelayMs = 50;
  const fluentRetryLimit = 40;

  function applyFluentTheme(theme, attempt){
    // A retry for a theme the visitor has since toggled away from stops here
    if(document.documentElement.getAttribute('data-osirion-theme') !== theme) return;

    const fluentTheme = window.Blazor && window.Blazor.theme;
    if(fluentTheme && typeof fluentTheme.setThemeMode === 'function'){
      if(fluentTheme.isDarkMode() !== (theme === 'dark')){
        fluentTheme.setThemeMode(theme);
      }
      return;
    }

    if(document.body){
      document.body.setAttribute('data-theme', theme);
    }

    if(attempt < fluentRetryLimit){
      setTimeout(function(){ applyFluentTheme(theme, attempt + 1); }, fluentRetryDelayMs);
    }
  }

  function wireUp(state){
    if(typeof Blazor !== 'undefined' && Blazor.addEventListener){
      Blazor.addEventListener('enhancedload', function(){
        applyTheme(state, resolveTheme(state));
      });
    }

    if(document.readyState === 'loading'){
      document.addEventListener('DOMContentLoaded', function(){
        applyTheme(state, resolveTheme(state));
      });
    } else {
      applyTheme(state, resolveTheme(state));
    }

    window.addEventListener('osirion-theme-update', function(){
      applyTheme(state, resolveTheme(state));
    });

    // Listen for system theme changes
    if (typeof window !== 'undefined' && window.matchMedia) {
      const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');
      const handleSystemThemeChange = function() {
        // Only react to system changes if no user preference is set
        const cookie = readCookie('osirion-preferred-theme');
        if (!cookie && state.config.defaultTheme === 'system') {
          applyTheme(state, resolveTheme(state));
        }
      };
      
      if (mediaQuery.addEventListener) {
        mediaQuery.addEventListener('change', handleSystemThemeChange);
      } else if (mediaQuery.addListener) {
        mediaQuery.addListener(handleSystemThemeChange);
      }
    }

    if(typeof MutationObserver !== 'undefined'){
      state.observer = new MutationObserver(function(mutations){
        let needsReapply = false;
        const html = document.documentElement;
        
        for(const mutation of mutations){
          if(mutation.type === 'attributes'){
            if(mutation.attributeName === 'data-osirion-theme'){
              const value = html.getAttribute(mutation.attributeName);
              if(!value || (value !== 'dark' && value !== 'light')){
                needsReapply = true;
                break;
              }
            }
            // Enhanced navigation copies the server's <html> attributes, which do not include the palette
            if(mutation.attributeName === 'data-osirion-palette'
              && (html.getAttribute('data-osirion-palette') || 'framework') !== resolvePalette(state)){
              needsReapply = true;
              break;
            }
            if(mutation.attributeName === 'data-bs-theme' && state.config.selectedFramework === 'Bootstrap'){
              const value = html.getAttribute(mutation.attributeName);
              if(!value){
                needsReapply = true;
                break;
              }
            }
          }
        }
        
        if(needsReapply){
          applyTheme(state, resolveTheme(state));
        }
      });
      
      state.observer.observe(document.documentElement, { 
        attributes: true, 
        attributeFilter: ['data-osirion-theme', 'data-osirion-palette', 'data-bs-theme', 'class']
      });
    }
  }

  window.initializeOsirionStyles = function(config){
    const state = ensureState();
        
    // Update configuration from server
    state.config.frameworkClass = config.frameworkClass || state.config.frameworkClass;
    state.config.selectedFramework = config.selectedFramework || state.config.selectedFramework;
    state.config.defaultTheme = config.defaultTheme || state.config.defaultTheme;
    state.config.currentTheme = config.currentTheme || state.config.currentTheme;
    state.config.defaultPalette = config.defaultPalette || state.config.defaultPalette;
    state.configReceived = true;

    if(!state.initialized){
      state.initialized = true;
      wireUp(state);
    } else {
      // Config updated - reapply theme with new defaults
      applyTheme(state, resolveTheme(state));
    }
  };

  function immediateBootstrap(){
    const html = document.documentElement;
    if(!html) return;

    // OsirionStyles already applied the configured framework and theme while the page was parsed
    if(html.hasAttribute('data-osirion-framework')) return;

    const cookie = readCookie('osirion-preferred-theme');
    
    // If user has a cookie preference, use it
    if(cookie) {
      const theme = normalizeTheme(cookie, 'light');
      html.setAttribute('data-osirion-theme', theme);
      html.setAttribute('data-osirion-framework', 'Bootstrap');
      html.setAttribute('data-bs-theme', theme);
      return;
    }
    
    // No cookie - apply a minimal safe theme that will be corrected by component initialization
    // Use light as safe default until server config arrives
    html.setAttribute('data-osirion-theme', 'light');
    html.setAttribute('data-osirion-framework', 'Bootstrap');
    html.setAttribute('data-bs-theme', 'light');
  }
  
  immediateBootstrap();
})();