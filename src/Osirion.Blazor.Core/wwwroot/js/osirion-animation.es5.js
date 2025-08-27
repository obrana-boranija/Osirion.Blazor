'use strict';

(function () {
    // Prevent multiple initializations
    if (window.osirionAosLoaded) return;
    window.osirionAosLoaded = true;

    // Function to load AOS library
    function loadAOS() {
        return new Promise(function (resolve, reject) {
            // Check if AOS is already available
            if (window.AOS) {
                resolve();
                return;
            }

            // Create CSS link if not already present
            if (!document.querySelector('link[href*="aos.css"]')) {
                var cssLink = document.createElement('link');
                cssLink.rel = 'stylesheet';
                cssLink.href = 'https://unpkg.com/aos@2.3.4/dist/aos.css';
                document.head.appendChild(cssLink);
            }

            // Create script element
            var script = document.createElement('script');
            script.src = 'https://unpkg.com/aos@2.3.4/dist/aos.js';
            script.onload = function () {
                return resolve();
            };
            script.onerror = function () {
                return reject(new Error('Failed to load AOS'));
            };
            document.head.appendChild(script);
        });
    }

    // Function to initialize AOS with proper scroll behavior
    function initializeAOS() {
        if (!window.AOS) return;

        try {
            (function () {
                var prefersReducedMotion = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)');

                // Always re-initialize to ensure proper state
                window.AOS.init({
                    // Animation settings
                    duration: 600,
                    easing: 'ease-out',
                    delay: 0,

                    // Scroll behavior - key fixes for proper scroll triggering
                    offset: 50, // Start animation 50px before element comes into view
                    anchorPlacement: 'top-bottom', // Trigger when top of element hits bottom of viewport
                    once: false, // Allow re-animation on scroll up/down
                    mirror: false, // Don't reverse animation when scrolling past

                    // Viewport detection
                    startEvent: 'DOMContentLoaded', // Wait for DOM to be ready
                    animatedClassName: 'aos-animate',
                    initClassName: 'aos-init',
                    useClassNames: false, // Use data attributes, not classes
                    disableMutationObserver: false, // Allow dynamic content

                    // Throttling for performance
                    debounceDelay: 50, // Throttle scroll events
                    throttleDelay: 99, // Throttle resize events

                    // Accessibility
                    disable: function disable() {
                        // Disable on mobile devices or when user prefers reduced motion
                        return window.innerWidth < 768 || prefersReducedMotion && prefersReducedMotion.matches;
                    }
                });
            })();
        } catch (error) {
            console.warn('AOS initialization failed:', error);
        }
    }

    // Function to handle enhanced navigation
    function handleEnhancedNavigation() {
        if (!window.AOS) return;

        // Reset all AOS elements to their initial state
        var aosElements = document.querySelectorAll('[data-aos]');
        aosElements.forEach(function (element) {
            element.classList.remove('aos-animate');
            element.classList.add('aos-init');
        });

        // Re-initialize AOS completely for new page content
        setTimeout(function () {
            initializeAOS();
            // Force refresh to detect new elements
            if (window.AOS.refresh) {
                window.AOS.refresh();
            }
        }, 50);
    }

    // Setup one-time event listeners
    function setupEventListeners() {
        // Prevent multiple event listener registrations
        if (window.osirionAosListenersSetup) return;
        window.osirionAosListenersSetup = true;

        // Handle reduced motion preference changes
        var prefersReducedMotion = window.matchMedia && window.matchMedia('(prefers-reduced-motion: reduce)');
        if (prefersReducedMotion) {
            prefersReducedMotion.addEventListener('change', function () {
                if (window.AOS && window.AOS.init) {
                    initializeAOS();
                }
            });
        }

        // Handle window resize for mobile responsiveness
        var resizeTimeout = undefined;
        window.addEventListener('resize', function () {
            clearTimeout(resizeTimeout);
            resizeTimeout = setTimeout(function () {
                if (window.AOS && window.AOS.refresh) {
                    window.AOS.refresh();
                }
            }, 150);
        });

        // Handle Blazor enhanced navigation - key fix for Static SSR
        if (window.Blazor) {
            window.Blazor.addEventListener('enhancedload', handleEnhancedNavigation);
        }

        // Expose utility functions globally
        window.osirionAosRefresh = function () {
            if (window.AOS && window.AOS.refresh) {
                window.AOS.refresh();
            }
        };

        window.osirionAosToggle = function (disable) {
            if (window.AOS && window.AOS.init) {
                initializeAOS();
            }
        };

        window.osirionAosReset = handleEnhancedNavigation;
    }

    // Main initialization function
    function initWhenReady() {
        loadAOS().then(function () {
            setupEventListeners();

            // Initialize immediately if DOM is ready
            if (document.readyState === 'complete') {
                setTimeout(function () {
                    initializeAOS();
                    // Force initial refresh
                    setTimeout(function () {
                        if (window.AOS && window.AOS.refresh) {
                            window.AOS.refresh();
                        }
                    }, 100);
                }, 50);
            } else {
                window.addEventListener('load', function () {
                    setTimeout(function () {
                        initializeAOS();
                        // Force initial refresh
                        setTimeout(function () {
                            if (window.AOS && window.AOS.refresh) {
                                window.AOS.refresh();
                            }
                        }, 100);
                    }, 50);
                });
            }
        })['catch'](function (error) {
            console.warn('Failed to load AOS library:', error);
        });
    }

    // Start initialization - allow re-initialization for enhanced navigation
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initWhenReady);
    } else {
        initWhenReady();
    }
})();

