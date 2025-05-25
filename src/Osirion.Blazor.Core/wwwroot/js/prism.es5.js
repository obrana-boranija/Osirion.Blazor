/* PrismJS 1.30.0
https://prismjs.com/download#themes=prism-okaidia&languages=markup+css+clike+javascript+aspnet+csharp+csv+docker+excel-formula+graphql+markdown+nginx+powershell+python+cshtml+jsx+tsx+regex+rest+sass+sql+iecst+t4-templating+t4-cs+typescript+vim+yaml&plugins=line-highlight+line-numbers+autolinker+toolbar+copy-to-clipboard */
"use strict";

var _self = "undefined" != typeof window ? window : "undefined" != typeof WorkerGlobalScope && self instanceof WorkerGlobalScope ? self : {},
    Prism = (function (e) {
  var n = /(?:^|\s)lang(?:uage)?-([\w-]+)(?=\s|$)/i,
      t = 0,
      r = {},
      a = { manual: e.Prism && e.Prism.manual, disableWorkerMessageHandler: e.Prism && e.Prism.disableWorkerMessageHandler, util: { encode: function e(n) {
        return n instanceof i ? new i(n.type, e(n.content), n.alias) : Array.isArray(n) ? n.map(e) : n.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/\u00a0/g, " ");
      }, type: function type(e) {
        return Object.prototype.toString.call(e).slice(8, -1);
      }, objId: function objId(e) {
        return e.__id || Object.defineProperty(e, "__id", { value: ++t }), e.__id;
      }, clone: function e(n, t) {
        var r, i;switch ((t = t || {}, a.util.type(n))) {case "Object":
            if ((i = a.util.objId(n), t[i])) return t[i];for (var l in (r = {}, t[i] = r, n)) n.hasOwnProperty(l) && (r[l] = e(n[l], t));return r;case "Array":
            return i = a.util.objId(n), t[i] ? t[i] : (r = [], t[i] = r, n.forEach(function (n, a) {
              r[a] = e(n, t);
            }), r);default:
            return n;}
      }, getLanguage: function getLanguage(e) {
        for (; e;) {
          var t = n.exec(e.className);if (t) return t[1].toLowerCase();e = e.parentElement;
        }return "none";
      }, setLanguage: function setLanguage(e, t) {
        e.className = e.className.replace(RegExp(n, "gi"), ""), e.classList.add("language-" + t);
      }, currentScript: function currentScript() {
        if ("undefined" == typeof document) return null;if (document.currentScript && "SCRIPT" === document.currentScript.tagName) return document.currentScript;try {
          throw new Error();
        } catch (r) {
          var e = (/at [^(\r\n]*\((.*):[^:]+:[^:]+\)$/i.exec(r.stack) || [])[1];if (e) {
            var n = document.getElementsByTagName("script");for (var t in n) if (n[t].src == e) return n[t];
          }return null;
        }
      }, isActive: function isActive(e, n, t) {
        for (var r = "no-" + n; e;) {
          var a = e.classList;if (a.contains(n)) return !0;if (a.contains(r)) return !1;e = e.parentElement;
        }return !!t;
      } }, languages: { plain: r, plaintext: r, text: r, txt: r, extend: function extend(e, n) {
        var t = a.util.clone(a.languages[e]);for (var r in n) t[r] = n[r];return t;
      }, insertBefore: function insertBefore(e, n, t, r) {
        var i = (r = r || a.languages)[e],
            l = {};for (var o in i) if (i.hasOwnProperty(o)) {
          if (o == n) for (var s in t) t.hasOwnProperty(s) && (l[s] = t[s]);t.hasOwnProperty(o) || (l[o] = i[o]);
        }var u = r[e];return r[e] = l, a.languages.DFS(a.languages, function (n, t) {
          t === u && n != e && (this[n] = l);
        }), l;
      }, DFS: function e(n, t, r, i) {
        i = i || {};var l = a.util.objId;for (var o in n) if (n.hasOwnProperty(o)) {
          t.call(n, o, n[o], r || o);var s = n[o],
              u = a.util.type(s);"Object" !== u || i[l(s)] ? "Array" !== u || i[l(s)] || (i[l(s)] = !0, e(s, t, o, i)) : (i[l(s)] = !0, e(s, t, null, i));
        }
      } }, plugins: {}, highlightAll: function highlightAll(e, n) {
      a.highlightAllUnder(document, e, n);
    }, highlightAllUnder: function highlightAllUnder(e, n, t) {
      var r = { callback: t, container: e, selector: 'code[class*="language-"], [class*="language-"] code, code[class*="lang-"], [class*="lang-"] code' };a.hooks.run("before-highlightall", r), r.elements = Array.prototype.slice.apply(r.container.querySelectorAll(r.selector)), a.hooks.run("before-all-elements-highlight", r);for (var i, l = 0; i = r.elements[l++];) a.highlightElement(i, !0 === n, r.callback);
    }, highlightElement: function highlightElement(n, t, r) {
      var i = a.util.getLanguage(n),
          l = a.languages[i];a.util.setLanguage(n, i);var o = n.parentElement;o && "pre" === o.nodeName.toLowerCase() && a.util.setLanguage(o, i);var s = { element: n, language: i, grammar: l, code: n.textContent };function u(e) {
        s.highlightedCode = e, a.hooks.run("before-insert", s), s.element.innerHTML = s.highlightedCode, a.hooks.run("after-highlight", s), a.hooks.run("complete", s), r && r.call(s.element);
      }if ((a.hooks.run("before-sanity-check", s), (o = s.element.parentElement) && "pre" === o.nodeName.toLowerCase() && !o.hasAttribute("tabindex") && o.setAttribute("tabindex", "0"), !s.code)) return a.hooks.run("complete", s), void (r && r.call(s.element));if ((a.hooks.run("before-highlight", s), s.grammar)) if (t && e.Worker) {
        var c = new Worker(a.filename);c.onmessage = function (e) {
          u(e.data);
        }, c.postMessage(JSON.stringify({ language: s.language, code: s.code, immediateClose: !0 }));
      } else u(a.highlight(s.code, s.grammar, s.language));else u(a.util.encode(s.code));
    }, highlight: function highlight(e, n, t) {
      var r = { code: e, grammar: n, language: t };if ((a.hooks.run("before-tokenize", r), !r.grammar)) throw new Error('The language "' + r.language + '" has no grammar.');return r.tokens = a.tokenize(r.code, r.grammar), a.hooks.run("after-tokenize", r), i.stringify(a.util.encode(r.tokens), r.language);
    }, tokenize: function tokenize(e, n) {
      var t = n.rest;if (t) {
        for (var r in t) n[r] = t[r];delete n.rest;
      }var a = new s();return u(a, a.head, e), o(e, a, n, a.head, 0), (function (e) {
        for (var n = [], t = e.head.next; t !== e.tail;) n.push(t.value), t = t.next;return n;
      })(a);
    }, hooks: { all: {}, add: function add(e, n) {
        var t = a.hooks.all;t[e] = t[e] || [], t[e].push(n);
      }, run: function run(e, n) {
        var t = a.hooks.all[e];if (t && t.length) for (var r, i = 0; r = t[i++];) r(n);
      } }, Token: i };function i(e, n, t, r) {
    this.type = e, this.content = n, this.alias = t, this.length = 0 | (r || "").length;
  }function l(e, n, t, r) {
    e.lastIndex = n;var a = e.exec(t);if (a && r && a[1]) {
      var i = a[1].length;a.index += i, a[0] = a[0].slice(i);
    }return a;
  }function o(e, n, t, r, s, g) {
    for (var f in t) if (t.hasOwnProperty(f) && t[f]) {
      var h = t[f];h = Array.isArray(h) ? h : [h];for (var d = 0; d < h.length; ++d) {
        if (g && g.cause == f + "," + d) return;var v = h[d],
            p = v.inside,
            m = !!v.lookbehind,
            y = !!v.greedy,
            k = v.alias;if (y && !v.pattern.global) {
          var x = v.pattern.toString().match(/[imsuy]*$/)[0];v.pattern = RegExp(v.pattern.source, x + "g");
        }for (var b = v.pattern || v, w = r.next, A = s; w !== n.tail && !(g && A >= g.reach); A += w.value.length, w = w.next) {
          var P = w.value;if (n.length > e.length) return;if (!(P instanceof i)) {
            var E,
                S = 1;if (y) {
              if (!(E = l(b, A, e, m)) || E.index >= e.length) break;var L = E.index,
                  O = E.index + E[0].length,
                  C = A;for (C += w.value.length; L >= C;) C += (w = w.next).value.length;if ((A = C -= w.value.length, w.value instanceof i)) continue;for (var j = w; j !== n.tail && (C < O || "string" == typeof j.value); j = j.next) S++, C += j.value.length;S--, P = e.slice(A, C), E.index -= A;
            } else if (!(E = l(b, 0, P, m))) continue;L = E.index;var N = E[0],
                _ = P.slice(0, L),
                M = P.slice(L + N.length),
                W = A + P.length;g && W > g.reach && (g.reach = W);var I = w.prev;if ((_ && (I = u(n, I, _), A += _.length), c(n, I, S), w = u(n, I, new i(f, p ? a.tokenize(N, p) : N, k, N)), M && u(n, w, M), S > 1)) {
              var T = { cause: f + "," + d, reach: W };o(e, n, t, w.prev, A, T), g && T.reach > g.reach && (g.reach = T.reach);
            }
          }
        }
      }
    }
  }function s() {
    var e = { value: null, prev: null, next: null },
        n = { value: null, prev: e, next: null };e.next = n, this.head = e, this.tail = n, this.length = 0;
  }function u(e, n, t) {
    var r = n.next,
        a = { value: t, prev: n, next: r };return n.next = a, r.prev = a, e.length++, a;
  }function c(e, n, t) {
    for (var r = n.next, a = 0; a < t && r !== e.tail; a++) r = r.next;n.next = r, r.prev = n, e.length -= a;
  }if ((e.Prism = a, i.stringify = function e(n, t) {
    if ("string" == typeof n) return n;if (Array.isArray(n)) {
      var r = "";return n.forEach(function (n) {
        r += e(n, t);
      }), r;
    }var i = { type: n.type, content: e(n.content, t), tag: "span", classes: ["token", n.type], attributes: {}, language: t },
        l = n.alias;l && (Array.isArray(l) ? Array.prototype.push.apply(i.classes, l) : i.classes.push(l)), a.hooks.run("wrap", i);var o = "";for (var s in i.attributes) o += " " + s + '="' + (i.attributes[s] || "").replace(/"/g, "&quot;") + '"';return "<" + i.tag + ' class="' + i.classes.join(" ") + '"' + o + ">" + i.content + "</" + i.tag + ">";
  }, !e.document)) return e.addEventListener ? (a.disableWorkerMessageHandler || e.addEventListener("message", function (n) {
    var t = JSON.parse(n.data),
        r = t.language,
        i = t.code,
        l = t.immediateClose;e.postMessage(a.highlight(i, a.languages[r], r)), l && e.close();
  }, !1), a) : a;var g = a.util.currentScript();function f() {
    a.manual || a.highlightAll();
  }if ((g && (a.filename = g.src, g.hasAttribute("data-manual") && (a.manual = !0)), !a.manual)) {
    var h = document.readyState;"loading" === h || "interactive" === h && g && g.defer ? document.addEventListener("DOMContentLoaded", f) : window.requestAnimationFrame ? window.requestAnimationFrame(f) : window.setTimeout(f, 16);
  }return a;
})(_self);"undefined" != typeof module && module.exports && (module.exports = Prism), "undefined" != typeof global && (global.Prism = Prism);
Prism.languages.markup = { comment: { pattern: /<!--(?:(?!<!--)[\s\S])*?-->/, greedy: !0 }, prolog: { pattern: /<\?[\s\S]+?\?>/, greedy: !0 }, doctype: { pattern: /<!DOCTYPE(?:[^>"'[\]]|"[^"]*"|'[^']*')+(?:\[(?:[^<"'\]]|"[^"]*"|'[^']*'|<(?!!--)|<!--(?:[^-]|-(?!->))*-->)*\]\s*)?>/i, greedy: !0, inside: { "internal-subset": { pattern: /(^[^\[]*\[)[\s\S]+(?=\]>$)/, lookbehind: !0, greedy: !0, inside: null }, string: { pattern: /"[^"]*"|'[^']*'/, greedy: !0 }, punctuation: /^<!|>$|[[\]]/, "doctype-tag": /^DOCTYPE/i, name: /[^\s<>'"]+/ } }, cdata: { pattern: /<!\[CDATA\[[\s\S]*?\]\]>/i, greedy: !0 }, tag: { pattern: /<\/?(?!\d)[^\s>\/=$<%]+(?:\s(?:\s*[^\s>\/=]+(?:\s*=\s*(?:"[^"]*"|'[^']*'|[^\s'">=]+(?=[\s>]))|(?=[\s/>])))+)?\s*\/?>/, greedy: !0, inside: { tag: { pattern: /^<\/?[^\s>\/]+/, inside: { punctuation: /^<\/?/, namespace: /^[^\s>\/:]+:/ } }, "special-attr": [], "attr-value": { pattern: /=\s*(?:"[^"]*"|'[^']*'|[^\s'">=]+)/, inside: { punctuation: [{ pattern: /^=/, alias: "attr-equals" }, { pattern: /^(\s*)["']|["']$/, lookbehind: !0 }] } }, punctuation: /\/?>/, "attr-name": { pattern: /[^\s>\/]+/, inside: { namespace: /^[^\s>\/:]+:/ } } } }, entity: [{ pattern: /&[\da-z]{1,8};/i, alias: "named-entity" }, /&#x?[\da-f]{1,8};/i] }, Prism.languages.markup.tag.inside["attr-value"].inside.entity = Prism.languages.markup.entity, Prism.languages.markup.doctype.inside["internal-subset"].inside = Prism.languages.markup, Prism.hooks.add("wrap", function (a) {
  "entity" === a.type && (a.attributes.title = a.content.replace(/&amp;/, "&"));
}), Object.defineProperty(Prism.languages.markup.tag, "addInlined", { value: function value(a, e) {
    var s = {};s["language-" + e] = { pattern: /(^<!\[CDATA\[)[\s\S]+?(?=\]\]>$)/i, lookbehind: !0, inside: Prism.languages[e] }, s.cdata = /^<!\[CDATA\[|\]\]>$/i;var t = { "included-cdata": { pattern: /<!\[CDATA\[[\s\S]*?\]\]>/i, inside: s } };t["language-" + e] = { pattern: /[\s\S]+/, inside: Prism.languages[e] };var n = {};n[a] = { pattern: RegExp("(<__[^>]*>)(?:<!\\[CDATA\\[(?:[^\\]]|\\](?!\\]>))*\\]\\]>|(?!<!\\[CDATA\\[)[^])*?(?=</__>)".replace(/__/g, function () {
        return a;
      }), "i"), lookbehind: !0, greedy: !0, inside: t }, Prism.languages.insertBefore("markup", "cdata", n);
  } }), Object.defineProperty(Prism.languages.markup.tag, "addAttribute", { value: function value(a, e) {
    Prism.languages.markup.tag.inside["special-attr"].push({ pattern: RegExp("(^|[\"'\\s])(?:" + a + ")\\s*=\\s*(?:\"[^\"]*\"|'[^']*'|[^\\s'\">=]+(?=[\\s>]))", "i"), lookbehind: !0, inside: { "attr-name": /^[^\s=]+/, "attr-value": { pattern: /=[\s\S]+/, inside: { value: { pattern: /(^=\s*(["']|(?!["'])))\S[\s\S]*(?=\2$)/, lookbehind: !0, alias: [e, "language-" + e], inside: Prism.languages[e] }, punctuation: [{ pattern: /^=/, alias: "attr-equals" }, /"|'/] } } } });
  } }), Prism.languages.html = Prism.languages.markup, Prism.languages.mathml = Prism.languages.markup, Prism.languages.svg = Prism.languages.markup, Prism.languages.xml = Prism.languages.extend("markup", {}), Prism.languages.ssml = Prism.languages.xml, Prism.languages.atom = Prism.languages.xml, Prism.languages.rss = Prism.languages.xml;
!(function (s) {
  var e = /(?:"(?:\\(?:\r\n|[\s\S])|[^"\\\r\n])*"|'(?:\\(?:\r\n|[\s\S])|[^'\\\r\n])*')/;s.languages.css = { comment: /\/\*[\s\S]*?\*\//, atrule: { pattern: RegExp("@[\\w-](?:[^;{\\s\"']|\\s+(?!\\s)|" + e.source + ")*?(?:;|(?=\\s*\\{))"), inside: { rule: /^@[\w-]+/, "selector-function-argument": { pattern: /(\bselector\s*\(\s*(?![\s)]))(?:[^()\s]|\s+(?![\s)])|\((?:[^()]|\([^()]*\))*\))+(?=\s*\))/, lookbehind: !0, alias: "selector" }, keyword: { pattern: /(^|[^\w-])(?:and|not|only|or)(?![\w-])/, lookbehind: !0 } } }, url: { pattern: RegExp("\\burl\\((?:" + e.source + "|(?:[^\\\\\r\n()\"']|\\\\[^])*)\\)", "i"), greedy: !0, inside: { "function": /^url/i, punctuation: /^\(|\)$/, string: { pattern: RegExp("^" + e.source + "$"), alias: "url" } } }, selector: { pattern: RegExp("(^|[{}\\s])[^{}\\s](?:[^{};\"'\\s]|\\s+(?![\\s{])|" + e.source + ")*(?=\\s*\\{)"), lookbehind: !0 }, string: { pattern: e, greedy: !0 }, property: { pattern: /(^|[^-\w\xA0-\uFFFF])(?!\s)[-_a-z\xA0-\uFFFF](?:(?!\s)[-\w\xA0-\uFFFF])*(?=\s*:)/i, lookbehind: !0 }, important: /!important\b/i, "function": { pattern: /(^|[^-a-z0-9])[-a-z0-9]+(?=\()/i, lookbehind: !0 }, punctuation: /[(){};:,]/ }, s.languages.css.atrule.inside.rest = s.languages.css;var t = s.languages.markup;t && (t.tag.addInlined("style", "css"), t.tag.addAttribute("style", "css"));
})(Prism);
Prism.languages.clike = { comment: [{ pattern: /(^|[^\\])\/\*[\s\S]*?(?:\*\/|$)/, lookbehind: !0, greedy: !0 }, { pattern: /(^|[^\\:])\/\/.*/, lookbehind: !0, greedy: !0 }], string: { pattern: /(["'])(?:\\(?:\r\n|[\s\S])|(?!\1)[^\\\r\n])*\1/, greedy: !0 }, "class-name": { pattern: /(\b(?:class|extends|implements|instanceof|interface|new|trait)\s+|\bcatch\s+\()[\w.\\]+/i, lookbehind: !0, inside: { punctuation: /[.\\]/ } }, keyword: /\b(?:break|catch|continue|do|else|finally|for|function|if|in|instanceof|new|null|return|throw|try|while)\b/, boolean: /\b(?:false|true)\b/, "function": /\b\w+(?=\()/, number: /\b0x[\da-f]+\b|(?:\b\d+(?:\.\d*)?|\B\.\d+)(?:e[+-]?\d+)?/i, operator: /[<>]=?|[!=]=?=?|--?|\+\+?|&&?|\|\|?|[?*/~^%]/, punctuation: /[{}[\];(),.:]/ };
Prism.languages.javascript = Prism.languages.extend("clike", { "class-name": [Prism.languages.clike["class-name"], { pattern: /(^|[^$\w\xA0-\uFFFF])(?!\s)[_$A-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*(?=\.(?:constructor|prototype))/, lookbehind: !0 }], keyword: [{ pattern: /((?:^|\})\s*)catch\b/, lookbehind: !0 }, { pattern: /(^|[^.]|\.\.\.\s*)\b(?:as|assert(?=\s*\{)|async(?=\s*(?:function\b|\(|[$\w\xA0-\uFFFF]|$))|await|break|case|class|const|continue|debugger|default|delete|do|else|enum|export|extends|finally(?=\s*(?:\{|$))|for|from(?=\s*(?:['"]|$))|function|(?:get|set)(?=\s*(?:[#\[$\w\xA0-\uFFFF]|$))|if|implements|import|in|instanceof|interface|let|new|null|of|package|private|protected|public|return|static|super|switch|this|throw|try|typeof|undefined|var|void|while|with|yield)\b/, lookbehind: !0 }], "function": /#?(?!\s)[_$a-zA-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*(?=\s*(?:\.\s*(?:apply|bind|call)\s*)?\()/, number: { pattern: RegExp("(^|[^\\w$])(?:NaN|Infinity|0[bB][01]+(?:_[01]+)*n?|0[oO][0-7]+(?:_[0-7]+)*n?|0[xX][\\dA-Fa-f]+(?:_[\\dA-Fa-f]+)*n?|\\d+(?:_\\d+)*n|(?:\\d+(?:_\\d+)*(?:\\.(?:\\d+(?:_\\d+)*)?)?|\\.\\d+(?:_\\d+)*)(?:[Ee][+-]?\\d+(?:_\\d+)*)?)(?![\\w$])"), lookbehind: !0 }, operator: /--|\+\+|\*\*=?|=>|&&=?|\|\|=?|[!=]==|<<=?|>>>?=?|[-+*/%&|^!=<>]=?|\.{3}|\?\?=?|\?\.?|[~:]/ }), Prism.languages.javascript["class-name"][0].pattern = /(\b(?:class|extends|implements|instanceof|interface|new)\s+)[\w.\\]+/, Prism.languages.insertBefore("javascript", "keyword", { regex: { pattern: RegExp("((?:^|[^$\\w\\xA0-\\uFFFF.\"'\\])\\s]|\\b(?:return|yield))\\s*)/(?:(?:\\[(?:[^\\]\\\\\r\n]|\\\\.)*\\]|\\\\.|[^/\\\\\\[\r\n])+/[dgimyus]{0,7}|(?:\\[(?:[^[\\]\\\\\r\n]|\\\\.|\\[(?:[^[\\]\\\\\r\n]|\\\\.|\\[(?:[^[\\]\\\\\r\n]|\\\\.)*\\])*\\])*\\]|\\\\.|[^/\\\\\\[\r\n])+/[dgimyus]{0,7}v[dgimyus]{0,7})(?=(?:\\s|/\\*(?:[^*]|\\*(?!/))*\\*/)*(?:$|[\r\n,.;:})\\]]|//))"), lookbehind: !0, greedy: !0, inside: { "regex-source": { pattern: /^(\/)[\s\S]+(?=\/[a-z]*$)/, lookbehind: !0, alias: "language-regex", inside: Prism.languages.regex }, "regex-delimiter": /^\/|\/$/, "regex-flags": /^[a-z]+$/ } }, "function-variable": { pattern: /#?(?!\s)[_$a-zA-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*(?=\s*[=:]\s*(?:async\s*)?(?:\bfunction\b|(?:\((?:[^()]|\([^()]*\))*\)|(?!\s)[_$a-zA-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*)\s*=>))/, alias: "function" }, parameter: [{ pattern: /(function(?:\s+(?!\s)[_$a-zA-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*)?\s*\(\s*)(?!\s)(?:[^()\s]|\s+(?![\s)])|\([^()]*\))+(?=\s*\))/, lookbehind: !0, inside: Prism.languages.javascript }, { pattern: /(^|[^$\w\xA0-\uFFFF])(?!\s)[_$a-z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*(?=\s*=>)/i, lookbehind: !0, inside: Prism.languages.javascript }, { pattern: /(\(\s*)(?!\s)(?:[^()\s]|\s+(?![\s)])|\([^()]*\))+(?=\s*\)\s*=>)/, lookbehind: !0, inside: Prism.languages.javascript }, { pattern: /((?:\b|\s|^)(?!(?:as|async|await|break|case|catch|class|const|continue|debugger|default|delete|do|else|enum|export|extends|finally|for|from|function|get|if|implements|import|in|instanceof|interface|let|new|null|of|package|private|protected|public|return|set|static|super|switch|this|throw|try|typeof|undefined|var|void|while|with|yield)(?![$\w\xA0-\uFFFF]))(?:(?!\s)[_$a-zA-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*\s*)\(\s*|\]\s*\(\s*)(?!\s)(?:[^()\s]|\s+(?![\s)])|\([^()]*\))+(?=\s*\)\s*\{)/, lookbehind: !0, inside: Prism.languages.javascript }], constant: /\b[A-Z](?:[A-Z_]|\dx?)*\b/ }), Prism.languages.insertBefore("javascript", "string", { hashbang: { pattern: /^#!.*/, greedy: !0, alias: "comment" }, "template-string": { pattern: /`(?:\\[\s\S]|\$\{(?:[^{}]|\{(?:[^{}]|\{[^}]*\})*\})+\}|(?!\$\{)[^\\`])*`/, greedy: !0, inside: { "template-punctuation": { pattern: /^`|`$/, alias: "string" }, interpolation: { pattern: /((?:^|[^\\])(?:\\{2})*)\$\{(?:[^{}]|\{(?:[^{}]|\{[^}]*\})*\})+\}/, lookbehind: !0, inside: { "interpolation-punctuation": { pattern: /^\$\{|\}$/, alias: "punctuation" }, rest: Prism.languages.javascript } }, string: /[\s\S]+/ } }, "string-property": { pattern: /((?:^|[,{])[ \t]*)(["'])(?:\\(?:\r\n|[\s\S])|(?!\2)[^\\\r\n])*\2(?=\s*:)/m, lookbehind: !0, greedy: !0, alias: "property" } }), Prism.languages.insertBefore("javascript", "operator", { "literal-property": { pattern: /((?:^|[,{])[ \t]*)(?!\s)[_$a-zA-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*(?=\s*:)/m, lookbehind: !0, alias: "property" } }), Prism.languages.markup && (Prism.languages.markup.tag.addInlined("script", "javascript"), Prism.languages.markup.tag.addAttribute("on(?:abort|blur|change|click|composition(?:end|start|update)|dblclick|error|focus(?:in|out)?|key(?:down|up)|load|mouse(?:down|enter|leave|move|out|over|up)|reset|resize|scroll|select|slotchange|submit|unload|wheel)", "javascript")), Prism.languages.js = Prism.languages.javascript;
!(function (e) {
  function n(e, n) {
    return e.replace(/<<(\d+)>>/g, function (e, s) {
      return "(?:" + n[+s] + ")";
    });
  }function s(e, s, a) {
    return RegExp(n(e, s), a || "");
  }function a(e, n) {
    for (var s = 0; s < n; s++) e = e.replace(/<<self>>/g, function () {
      return "(?:" + e + ")";
    });return e.replace(/<<self>>/g, "[^\\s\\S]");
  }var t = "bool byte char decimal double dynamic float int long object sbyte short string uint ulong ushort var void",
      r = "class enum interface record struct",
      i = "add alias and ascending async await by descending from(?=\\s*(?:\\w|$)) get global group into init(?=\\s*;) join let nameof not notnull on or orderby partial remove select set unmanaged value when where with(?=\\s*{)",
      o = "abstract as base break case catch checked const continue default delegate do else event explicit extern finally fixed for foreach goto if implicit in internal is lock namespace new null operator out override params private protected public readonly ref return sealed sizeof stackalloc static switch this throw try typeof unchecked unsafe using virtual volatile while yield";function l(e) {
    return "\\b(?:" + e.trim().replace(/ /g, "|") + ")\\b";
  }var d = l(r),
      p = RegExp(l(t + " " + r + " " + i + " " + o)),
      c = l(r + " " + i + " " + o),
      u = l(t + " " + r + " " + o),
      g = a("<(?:[^<>;=+\\-*/%&|^]|<<self>>)*>", 2),
      b = a("\\((?:[^()]|<<self>>)*\\)", 2),
      h = "@?\\b[A-Za-z_]\\w*\\b",
      f = n("<<0>>(?:\\s*<<1>>)?", [h, g]),
      m = n("(?!<<0>>)<<1>>(?:\\s*\\.\\s*<<1>>)*", [c, f]),
      k = "\\[\\s*(?:,\\s*)*\\]",
      y = n("<<0>>(?:\\s*(?:\\?\\s*)?<<1>>)*(?:\\s*\\?)?", [m, k]),
      w = n("[^,()<>[\\];=+\\-*/%&|^]|<<0>>|<<1>>|<<2>>", [g, b, k]),
      v = n("\\(<<0>>+(?:,<<0>>+)+\\)", [w]),
      x = n("(?:<<0>>|<<1>>)(?:\\s*(?:\\?\\s*)?<<2>>)*(?:\\s*\\?)?", [v, m, k]),
      $ = { keyword: p, punctuation: /[<>()?,.:[\]]/ },
      _ = "'(?:[^\r\n'\\\\]|\\\\.|\\\\[Uux][\\da-fA-F]{1,8})'",
      B = '"(?:\\\\.|[^\\\\"\r\n])*"';e.languages.csharp = e.languages.extend("clike", { string: [{ pattern: s("(^|[^$\\\\])<<0>>", ['@"(?:""|\\\\[^]|[^\\\\"])*"(?!")']), lookbehind: !0, greedy: !0 }, { pattern: s("(^|[^@$\\\\])<<0>>", [B]), lookbehind: !0, greedy: !0 }], "class-name": [{ pattern: s("(\\busing\\s+static\\s+)<<0>>(?=\\s*;)", [m]), lookbehind: !0, inside: $ }, { pattern: s("(\\busing\\s+<<0>>\\s*=\\s*)<<1>>(?=\\s*;)", [h, x]), lookbehind: !0, inside: $ }, { pattern: s("(\\busing\\s+)<<0>>(?=\\s*=)", [h]), lookbehind: !0 }, { pattern: s("(\\b<<0>>\\s+)<<1>>", [d, f]), lookbehind: !0, inside: $ }, { pattern: s("(\\bcatch\\s*\\(\\s*)<<0>>", [m]), lookbehind: !0, inside: $ }, { pattern: s("(\\bwhere\\s+)<<0>>", [h]), lookbehind: !0 }, { pattern: s("(\\b(?:is(?:\\s+not)?|as)\\s+)<<0>>", [y]), lookbehind: !0, inside: $ }, { pattern: s("\\b<<0>>(?=\\s+(?!<<1>>|with\\s*\\{)<<2>>(?:\\s*[=,;:{)\\]]|\\s+(?:in|when)\\b))", [x, u, h]), inside: $ }], keyword: p, number: /(?:\b0(?:x[\da-f_]*[\da-f]|b[01_]*[01])|(?:\B\.\d+(?:_+\d+)*|\b\d+(?:_+\d+)*(?:\.\d+(?:_+\d+)*)?)(?:e[-+]?\d+(?:_+\d+)*)?)(?:[dflmu]|lu|ul)?\b/i, operator: />>=?|<<=?|[-=]>|([-+&|])\1|~|\?\?=?|[-+*/%&|^!=<>]=?/, punctuation: /\?\.?|::|[{}[\];(),.:]/ }), e.languages.insertBefore("csharp", "number", { range: { pattern: /\.\./, alias: "operator" } }), e.languages.insertBefore("csharp", "punctuation", { "named-parameter": { pattern: s("([(,]\\s*)<<0>>(?=\\s*:)", [h]), lookbehind: !0, alias: "punctuation" } }), e.languages.insertBefore("csharp", "class-name", { namespace: { pattern: s("(\\b(?:namespace|using)\\s+)<<0>>(?:\\s*\\.\\s*<<0>>)*(?=\\s*[;{])", [h]), lookbehind: !0, inside: { punctuation: /\./ } }, "type-expression": { pattern: s("(\\b(?:default|sizeof|typeof)\\s*\\(\\s*(?!\\s))(?:[^()\\s]|\\s(?!\\s)|<<0>>)*(?=\\s*\\))", [b]), lookbehind: !0, alias: "class-name", inside: $ }, "return-type": { pattern: s("<<0>>(?=\\s+(?:<<1>>\\s*(?:=>|[({]|\\.\\s*this\\s*\\[)|this\\s*\\[))", [x, m]), inside: $, alias: "class-name" }, "constructor-invocation": { pattern: s("(\\bnew\\s+)<<0>>(?=\\s*[[({])", [x]), lookbehind: !0, inside: $, alias: "class-name" }, "generic-method": { pattern: s("<<0>>\\s*<<1>>(?=\\s*\\()", [h, g]), inside: { "function": s("^<<0>>", [h]), generic: { pattern: RegExp(g), alias: "class-name", inside: $ } } }, "type-list": { pattern: s("\\b((?:<<0>>\\s+<<1>>|record\\s+<<1>>\\s*<<5>>|where\\s+<<2>>)\\s*:\\s*)(?:<<3>>|<<4>>|<<1>>\\s*<<5>>|<<6>>)(?:\\s*,\\s*(?:<<3>>|<<4>>|<<6>>))*(?=\\s*(?:where|[{;]|=>|$))", [d, f, h, x, p.source, b, "\\bnew\\s*\\(\\s*\\)"]), lookbehind: !0, inside: { "record-arguments": { pattern: s("(^(?!new\\s*\\()<<0>>\\s*)<<1>>", [f, b]), lookbehind: !0, greedy: !0, inside: e.languages.csharp }, keyword: p, "class-name": { pattern: RegExp(x), greedy: !0, inside: $ }, punctuation: /[,()]/ } }, preprocessor: { pattern: /(^[\t ]*)#.*/m, lookbehind: !0, alias: "property", inside: { directive: { pattern: /(#)\b(?:define|elif|else|endif|endregion|error|if|line|nullable|pragma|region|undef|warning)\b/, lookbehind: !0, alias: "keyword" } } } });var E = B + "|" + _,
      R = n("/(?![*/])|//[^\r\n]*[\r\n]|/\\*(?:[^*]|\\*(?!/))*\\*/|<<0>>", [E]),
      z = a(n("[^\"'/()]|<<0>>|\\(<<self>>*\\)", [R]), 2),
      S = "\\b(?:assembly|event|field|method|module|param|property|return|type)\\b",
      j = n("<<0>>(?:\\s*\\(<<1>>*\\))?", [m, z]);e.languages.insertBefore("csharp", "class-name", { attribute: { pattern: s("((?:^|[^\\s\\w>)?])\\s*\\[\\s*)(?:<<0>>\\s*:\\s*)?<<1>>(?:\\s*,\\s*<<1>>)*(?=\\s*\\])", [S, j]), lookbehind: !0, greedy: !0, inside: { target: { pattern: s("^<<0>>(?=\\s*:)", [S]), alias: "keyword" }, "attribute-arguments": { pattern: s("\\(<<0>>*\\)", [z]), inside: e.languages.csharp }, "class-name": { pattern: RegExp(m), inside: { punctuation: /\./ } }, punctuation: /[:,]/ } } });var A = ":[^}\r\n]+",
      F = a(n("[^\"'/()]|<<0>>|\\(<<self>>*\\)", [R]), 2),
      P = n("\\{(?!\\{)(?:(?![}:])<<0>>)*<<1>>?\\}", [F, A]),
      U = a(n("[^\"'/()]|/(?!\\*)|/\\*(?:[^*]|\\*(?!/))*\\*/|<<0>>|\\(<<self>>*\\)", [E]), 2),
      Z = n("\\{(?!\\{)(?:(?![}:])<<0>>)*<<1>>?\\}", [U, A]);function q(n, a) {
    return { interpolation: { pattern: s("((?:^|[^{])(?:\\{\\{)*)<<0>>", [n]), lookbehind: !0, inside: { "format-string": { pattern: s("(^\\{(?:(?![}:])<<0>>)*)<<1>>(?=\\}$)", [a, A]), lookbehind: !0, inside: { punctuation: /^:/ } }, punctuation: /^\{|\}$/, expression: { pattern: /[\s\S]+/, alias: "language-csharp", inside: e.languages.csharp } } }, string: /[\s\S]+/ };
  }e.languages.insertBefore("csharp", "string", { "interpolation-string": [{ pattern: s('(^|[^\\\\])(?:\\$@|@\\$)"(?:""|\\\\[^]|\\{\\{|<<0>>|[^\\\\{"])*"', [P]), lookbehind: !0, greedy: !0, inside: q(P, F) }, { pattern: s('(^|[^@\\\\])\\$"(?:\\\\.|\\{\\{|<<0>>|[^\\\\"{])*"', [Z]), lookbehind: !0, greedy: !0, inside: q(Z, U) }], char: { pattern: RegExp(_), greedy: !0 } }), e.languages.dotnet = e.languages.cs = e.languages.csharp;
})(Prism);
Prism.languages.aspnet = Prism.languages.extend("markup", { "page-directive": { pattern: /<%\s*@.*%>/, alias: "tag", inside: { "page-directive": { pattern: /<%\s*@\s*(?:Assembly|Control|Implements|Import|Master(?:Type)?|OutputCache|Page|PreviousPageType|Reference|Register)?|%>/i, alias: "tag" }, rest: Prism.languages.markup.tag.inside } }, directive: { pattern: /<%.*%>/, alias: "tag", inside: { directive: { pattern: /<%\s*?[$=%#:]{0,2}|%>/, alias: "tag" }, rest: Prism.languages.csharp } } }), Prism.languages.aspnet.tag.pattern = /<(?!%)\/?[^\s>\/]+(?:\s+[^\s>\/=]+(?:=(?:("|')(?:\\[\s\S]|(?!\1)[^\\])*\1|[^\s'">=]+))?)*\s*\/?>/, Prism.languages.insertBefore("inside", "punctuation", { directive: Prism.languages.aspnet.directive }, Prism.languages.aspnet.tag.inside["attr-value"]), Prism.languages.insertBefore("aspnet", "comment", { "asp-comment": { pattern: /<%--[\s\S]*?--%>/, alias: ["asp", "comment"] } }), Prism.languages.insertBefore("aspnet", Prism.languages.javascript ? "script" : "tag", { "asp-script": { pattern: /(<script(?=.*runat=['"]?server\b)[^>]*>)[\s\S]*?(?=<\/script>)/i, lookbehind: !0, alias: ["asp", "script"], inside: Prism.languages.csharp || {} } });
Prism.languages.csv = { value: /[^\r\n,"]+|"(?:[^"]|"")*"(?!")/, punctuation: /,/ };
!(function (e) {
  var n = "(?:[ \t]+(?![ \t])(?:<SP_BS>)?|<SP_BS>)".replace(/<SP_BS>/g, function () {
    return "\\\\[\r\n](?:\\s|\\\\[\r\n]|#.*(?!.))*(?![\\s#]|\\\\[\r\n])";
  }),
      r = "\"(?:[^\"\\\\\r\n]|\\\\(?:\r\n|[^]))*\"|'(?:[^'\\\\\r\n]|\\\\(?:\r\n|[^]))*'",
      t = "--[\\w-]+=(?:<STR>|(?![\"'])(?:[^\\s\\\\]|\\\\.)+)".replace(/<STR>/g, function () {
    return r;
  }),
      o = { pattern: RegExp(r), greedy: !0 },
      i = { pattern: /(^[ \t]*)#.*/m, lookbehind: !0, greedy: !0 };function a(e, r) {
    return e = e.replace(/<OPT>/g, function () {
      return t;
    }).replace(/<SP>/g, function () {
      return n;
    }), RegExp(e, r);
  }e.languages.docker = { instruction: { pattern: /(^[ \t]*)(?:ADD|ARG|CMD|COPY|ENTRYPOINT|ENV|EXPOSE|FROM|HEALTHCHECK|LABEL|MAINTAINER|ONBUILD|RUN|SHELL|STOPSIGNAL|USER|VOLUME|WORKDIR)(?=\s)(?:\\.|[^\r\n\\])*(?:\\$(?:\s|#.*$)*(?![\s#])(?:\\.|[^\r\n\\])*)*/im, lookbehind: !0, greedy: !0, inside: { options: { pattern: a("(^(?:ONBUILD<SP>)?\\w+<SP>)<OPT>(?:<SP><OPT>)*", "i"), lookbehind: !0, greedy: !0, inside: { property: { pattern: /(^|\s)--[\w-]+/, lookbehind: !0 }, string: [o, { pattern: /(=)(?!["'])(?:[^\s\\]|\\.)+/, lookbehind: !0 }], operator: /\\$/m, punctuation: /=/ } }, keyword: [{ pattern: a("(^(?:ONBUILD<SP>)?HEALTHCHECK<SP>(?:<OPT><SP>)*)(?:CMD|NONE)\\b", "i"), lookbehind: !0, greedy: !0 }, { pattern: a("(^(?:ONBUILD<SP>)?FROM<SP>(?:<OPT><SP>)*(?!--)[^ \t\\\\]+<SP>)AS", "i"), lookbehind: !0, greedy: !0 }, { pattern: a("(^ONBUILD<SP>)\\w+", "i"), lookbehind: !0, greedy: !0 }, { pattern: /^\w+/, greedy: !0 }], comment: i, string: o, variable: /\$(?:\w+|\{[^{}"'\\]*\})/, operator: /\\$/m } }, comment: i }, e.languages.dockerfile = e.languages.docker;
})(Prism);
Prism.languages["excel-formula"] = { comment: { pattern: /(\bN\(\s*)"(?:[^"]|"")*"(?=\s*\))/i, lookbehind: !0, greedy: !0 }, string: { pattern: /"(?:[^"]|"")*"(?!")/, greedy: !0 }, reference: { pattern: /(?:'[^']*'|(?:[^\s()[\]{}<>*?"';,$&]*\[[^^\s()[\]{}<>*?"']+\])?\w+)!/, greedy: !0, alias: "string", inside: { operator: /!$/, punctuation: /'/, sheet: { pattern: /[^[\]]+$/, alias: "function" }, file: { pattern: /\[[^[\]]+\]$/, inside: { punctuation: /[[\]]/ } }, path: /[\s\S]+/ } }, "function-name": { pattern: /\b[A-Z]\w*(?=\()/i, alias: "builtin" }, range: { pattern: /\$?\b(?:[A-Z]+\$?\d+:\$?[A-Z]+\$?\d+|[A-Z]+:\$?[A-Z]+|\d+:\$?\d+)\b/i, alias: "selector", inside: { operator: /:/, cell: /\$?[A-Z]+\$?\d+/i, column: /\$?[A-Z]+/i, row: /\$?\d+/ } }, cell: { pattern: /\b[A-Z]+\d+\b|\$[A-Za-z]+\$?\d+\b|\b[A-Za-z]+\$\d+\b/, alias: "selector" }, number: /(?:\b\d+(?:\.\d+)?|\B\.\d+)(?:e[+-]?\d+)?\b/i, boolean: /\b(?:FALSE|TRUE)\b/i, operator: /[-+*/^%=&,]|<[=>]?|>=?/, punctuation: /[[\]();{}|]/ }, Prism.languages.xlsx = Prism.languages.xls = Prism.languages["excel-formula"];
Prism.languages.graphql = { comment: /#.*/, description: { pattern: /(?:"""(?:[^"]|(?!""")")*"""|"(?:\\.|[^\\"\r\n])*")(?=\s*[a-z_])/i, greedy: !0, alias: "string", inside: { "language-markdown": { pattern: /(^"(?:"")?)(?!\1)[\s\S]+(?=\1$)/, lookbehind: !0, inside: Prism.languages.markdown } } }, string: { pattern: /"""(?:[^"]|(?!""")")*"""|"(?:\\.|[^\\"\r\n])*"/, greedy: !0 }, number: /(?:\B-|\b)\d+(?:\.\d+)?(?:e[+-]?\d+)?\b/i, boolean: /\b(?:false|true)\b/, variable: /\$[a-z_]\w*/i, directive: { pattern: /@[a-z_]\w*/i, alias: "function" }, "attr-name": { pattern: /\b[a-z_]\w*(?=\s*(?:\((?:[^()"]|"(?:\\.|[^\\"\r\n])*")*\))?:)/i, greedy: !0 }, "atom-input": { pattern: /\b[A-Z]\w*Input\b/, alias: "class-name" }, scalar: /\b(?:Boolean|Float|ID|Int|String)\b/, constant: /\b[A-Z][A-Z_\d]*\b/, "class-name": { pattern: /(\b(?:enum|implements|interface|on|scalar|type|union)\s+|&\s*|:\s*|\[)[A-Z_]\w*/, lookbehind: !0 }, fragment: { pattern: /(\bfragment\s+|\.{3}\s*(?!on\b))[a-zA-Z_]\w*/, lookbehind: !0, alias: "function" }, "definition-mutation": { pattern: /(\bmutation\s+)[a-zA-Z_]\w*/, lookbehind: !0, alias: "function" }, "definition-query": { pattern: /(\bquery\s+)[a-zA-Z_]\w*/, lookbehind: !0, alias: "function" }, keyword: /\b(?:directive|enum|extend|fragment|implements|input|interface|mutation|on|query|repeatable|scalar|schema|subscription|type|union)\b/, operator: /[!=|&]|\.{3}/, "property-query": /\w+(?=\s*\()/, object: /\w+(?=\s*\{)/, punctuation: /[!(){}\[\]:=,]/, property: /\w+/ }, Prism.hooks.add("after-tokenize", function (n) {
  if ("graphql" === n.language) for (var t = n.tokens.filter(function (n) {
    return "string" != typeof n && "comment" !== n.type && "scalar" !== n.type;
  }), e = 0; e < t.length;) {
    var a = t[e++];if ("keyword" === a.type && "mutation" === a.content) {
      var r = [];if (c(["definition-mutation", "punctuation"]) && "(" === l(1).content) {
        e += 2;var i = f(/^\($/, /^\)$/);if (-1 === i) continue;for (; e < i; e++) {
          var o = l(0);"variable" === o.type && (b(o, "variable-input"), r.push(o.content));
        }e = i + 1;
      }if (c(["punctuation", "property-query"]) && "{" === l(0).content && (e++, b(l(0), "property-mutation"), r.length > 0)) {
        var s = f(/^\{$/, /^\}$/);if (-1 === s) continue;for (var u = e; u < s; u++) {
          var p = t[u];"variable" === p.type && r.indexOf(p.content) >= 0 && b(p, "variable-input");
        }
      }
    }
  }function l(n) {
    return t[e + n];
  }function c(n, t) {
    t = t || 0;for (var e = 0; e < n.length; e++) {
      var a = l(e + t);if (!a || a.type !== n[e]) return !1;
    }return !0;
  }function f(n, a) {
    for (var r = 1, i = e; i < t.length; i++) {
      var o = t[i],
          s = o.content;if ("punctuation" === o.type && "string" == typeof s) if (n.test(s)) r++;else if (a.test(s) && 0 == --r) return i;
    }return -1;
  }function b(n, t) {
    var e = n.alias;e ? Array.isArray(e) || (n.alias = e = [e]) : n.alias = e = [], e.push(t);
  }
});
!(function (n) {
  function e(n) {
    return n = n.replace(/<inner>/g, function () {
      return "(?:\\\\.|[^\\\\\n\r]|(?:\n|\r\n?)(?![\r\n]))";
    }), RegExp("((?:^|[^\\\\])(?:\\\\{2})*)(?:" + n + ")");
  }var t = "(?:\\\\.|``(?:[^`\r\n]|`(?!`))+``|`[^`\r\n]+`|[^\\\\|\r\n`])+",
      a = "\\|?__(?:\\|__)+\\|?(?:(?:\n|\r\n?)|(?![^]))".replace(/__/g, function () {
    return t;
  }),
      i = "\\|?[ \t]*:?-{3,}:?[ \t]*(?:\\|[ \t]*:?-{3,}:?[ \t]*)+\\|?(?:\n|\r\n?)";n.languages.markdown = n.languages.extend("markup", {}), n.languages.insertBefore("markdown", "prolog", { "front-matter-block": { pattern: /(^(?:\s*[\r\n])?)---(?!.)[\s\S]*?[\r\n]---(?!.)/, lookbehind: !0, greedy: !0, inside: { punctuation: /^---|---$/, "front-matter": { pattern: /\S+(?:\s+\S+)*/, alias: ["yaml", "language-yaml"], inside: n.languages.yaml } } }, blockquote: { pattern: /^>(?:[\t ]*>)*/m, alias: "punctuation" }, table: { pattern: RegExp("^" + a + i + "(?:" + a + ")*", "m"), inside: { "table-data-rows": { pattern: RegExp("^(" + a + i + ")(?:" + a + ")*$"), lookbehind: !0, inside: { "table-data": { pattern: RegExp(t), inside: n.languages.markdown }, punctuation: /\|/ } }, "table-line": { pattern: RegExp("^(" + a + ")" + i + "$"), lookbehind: !0, inside: { punctuation: /\||:?-{3,}:?/ } }, "table-header-row": { pattern: RegExp("^" + a + "$"), inside: { "table-header": { pattern: RegExp(t), alias: "important", inside: n.languages.markdown }, punctuation: /\|/ } } } }, code: [{ pattern: /((?:^|\n)[ \t]*\n|(?:^|\r\n?)[ \t]*\r\n?)(?: {4}|\t).+(?:(?:\n|\r\n?)(?: {4}|\t).+)*/, lookbehind: !0, alias: "keyword" }, { pattern: /^```[\s\S]*?^```$/m, greedy: !0, inside: { "code-block": { pattern: /^(```.*(?:\n|\r\n?))[\s\S]+?(?=(?:\n|\r\n?)^```$)/m, lookbehind: !0 }, "code-language": { pattern: /^(```).+/, lookbehind: !0 }, punctuation: /```/ } }], title: [{ pattern: /\S.*(?:\n|\r\n?)(?:==+|--+)(?=[ \t]*$)/m, alias: "important", inside: { punctuation: /==+$|--+$/ } }, { pattern: /(^\s*)#.+/m, lookbehind: !0, alias: "important", inside: { punctuation: /^#+|#+$/ } }], hr: { pattern: /(^\s*)([*-])(?:[\t ]*\2){2,}(?=\s*$)/m, lookbehind: !0, alias: "punctuation" }, list: { pattern: /(^\s*)(?:[*+-]|\d+\.)(?=[\t ].)/m, lookbehind: !0, alias: "punctuation" }, "url-reference": { pattern: /!?\[[^\]]+\]:[\t ]+(?:\S+|<(?:\\.|[^>\\])+>)(?:[\t ]+(?:"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*'|\((?:\\.|[^)\\])*\)))?/, inside: { variable: { pattern: /^(!?\[)[^\]]+/, lookbehind: !0 }, string: /(?:"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*'|\((?:\\.|[^)\\])*\))$/, punctuation: /^[\[\]!:]|[<>]/ }, alias: "url" }, bold: { pattern: e("\\b__(?:(?!_)<inner>|_(?:(?!_)<inner>)+_)+__\\b|\\*\\*(?:(?!\\*)<inner>|\\*(?:(?!\\*)<inner>)+\\*)+\\*\\*"), lookbehind: !0, greedy: !0, inside: { content: { pattern: /(^..)[\s\S]+(?=..$)/, lookbehind: !0, inside: {} }, punctuation: /\*\*|__/ } }, italic: { pattern: e("\\b_(?:(?!_)<inner>|__(?:(?!_)<inner>)+__)+_\\b|\\*(?:(?!\\*)<inner>|\\*\\*(?:(?!\\*)<inner>)+\\*\\*)+\\*"), lookbehind: !0, greedy: !0, inside: { content: { pattern: /(^.)[\s\S]+(?=.$)/, lookbehind: !0, inside: {} }, punctuation: /[*_]/ } }, strike: { pattern: e("(~~?)(?:(?!~)<inner>)+\\2"), lookbehind: !0, greedy: !0, inside: { content: { pattern: /(^~~?)[\s\S]+(?=\1$)/, lookbehind: !0, inside: {} }, punctuation: /~~?/ } }, "code-snippet": { pattern: /(^|[^\\`])(?:``[^`\r\n]+(?:`[^`\r\n]+)*``(?!`)|`[^`\r\n]+`(?!`))/, lookbehind: !0, greedy: !0, alias: ["code", "keyword"] }, url: { pattern: e('!?\\[(?:(?!\\])<inner>)+\\](?:\\([^\\s)]+(?:[\t ]+"(?:\\\\.|[^"\\\\])*")?\\)|[ \t]?\\[(?:(?!\\])<inner>)+\\])'), lookbehind: !0, greedy: !0, inside: { operator: /^!/, content: { pattern: /(^\[)[^\]]+(?=\])/, lookbehind: !0, inside: {} }, variable: { pattern: /(^\][ \t]?\[)[^\]]+(?=\]$)/, lookbehind: !0 }, url: { pattern: /(^\]\()[^\s)]+/, lookbehind: !0 }, string: { pattern: /(^[ \t]+)"(?:\\.|[^"\\])*"(?=\)$)/, lookbehind: !0 } } } }), ["url", "bold", "italic", "strike"].forEach(function (e) {
    ["url", "bold", "italic", "strike", "code-snippet"].forEach(function (t) {
      e !== t && (n.languages.markdown[e].inside.content.inside[t] = n.languages.markdown[t]);
    });
  }), n.hooks.add("after-tokenize", function (n) {
    "markdown" !== n.language && "md" !== n.language || (function n(e) {
      if (e && "string" != typeof e) for (var t = 0, a = e.length; t < a; t++) {
        var i = e[t];if ("code" === i.type) {
          var r = i.content[1],
              o = i.content[3];if (r && o && "code-language" === r.type && "code-block" === o.type && "string" == typeof r.content) {
            var l = r.content.replace(/\b#/g, "sharp").replace(/\b\+\+/g, "pp"),
                s = "language-" + (l = (/[a-z][\w-]*/i.exec(l) || [""])[0].toLowerCase());o.alias ? "string" == typeof o.alias ? o.alias = [o.alias, s] : o.alias.push(s) : o.alias = [s];
          }
        } else n(i.content);
      }
    })(n.tokens);
  }), n.hooks.add("wrap", function (e) {
    if ("code-block" === e.type) {
      for (var t = "", a = 0, i = e.classes.length; a < i; a++) {
        var s = e.classes[a],
            d = /language-(.+)/.exec(s);if (d) {
          t = d[1];break;
        }
      }var p = n.languages[t];if (p) e.content = n.highlight(e.content.replace(r, "").replace(/&(\w{1,8}|#x?[\da-f]{1,8});/gi, function (n, e) {
        var t;return "#" === (e = e.toLowerCase())[0] ? (t = "x" === e[1] ? parseInt(e.slice(2), 16) : Number(e.slice(1)), l(t)) : o[e] || n;
      }), p, t);else if (t && "none" !== t && n.plugins.autoloader) {
        var u = "md-" + new Date().valueOf() + "-" + Math.floor(1e16 * Math.random());e.attributes.id = u, n.plugins.autoloader.loadLanguages(t, function () {
          var e = document.getElementById(u);e && (e.innerHTML = n.highlight(e.textContent, n.languages[t], t));
        });
      }
    }
  });var r = RegExp(n.languages.markup.tag.pattern.source, "gi"),
      o = { amp: "&", lt: "<", gt: ">", quot: '"' },
      l = String.fromCodePoint || String.fromCharCode;n.languages.md = n.languages.markdown;
})(Prism);
!(function (e) {
  var n = /\$(?:\w[a-z\d]*(?:_[^\x00-\x1F\s"'\\()$]*)?|\{[^}\s"'\\]+\})/i;e.languages.nginx = { comment: { pattern: /(^|[\s{};])#.*/, lookbehind: !0, greedy: !0 }, directive: { pattern: /(^|\s)\w(?:[^;{}"'\\\s]|\\.|"(?:[^"\\]|\\.)*"|'(?:[^'\\]|\\.)*'|\s+(?:#.*(?!.)|(?![#\s])))*?(?=\s*[;{])/, lookbehind: !0, greedy: !0, inside: { string: { pattern: /((?:^|[^\\])(?:\\\\)*)(?:"(?:[^"\\]|\\.)*"|'(?:[^'\\]|\\.)*')/, lookbehind: !0, greedy: !0, inside: { escape: { pattern: /\\["'\\nrt]/, alias: "entity" }, variable: n } }, comment: { pattern: /(\s)#.*/, lookbehind: !0, greedy: !0 }, keyword: { pattern: /^\S+/, greedy: !0 }, boolean: { pattern: /(\s)(?:off|on)(?!\S)/, lookbehind: !0 }, number: { pattern: /(\s)\d+[a-z]*(?!\S)/i, lookbehind: !0 }, variable: n } }, punctuation: /[{};]/ };
})(Prism);
!(function (e) {
  var i = e.languages.powershell = { comment: [{ pattern: /(^|[^`])<#[\s\S]*?#>/, lookbehind: !0 }, { pattern: /(^|[^`])#.*/, lookbehind: !0 }], string: [{ pattern: /"(?:`[\s\S]|[^`"])*"/, greedy: !0, inside: null }, { pattern: /'(?:[^']|'')*'/, greedy: !0 }], namespace: /\[[a-z](?:\[(?:\[[^\]]*\]|[^\[\]])*\]|[^\[\]])*\]/i, boolean: /\$(?:false|true)\b/i, variable: /\$\w+\b/, "function": [/\b(?:Add|Approve|Assert|Backup|Block|Checkpoint|Clear|Close|Compare|Complete|Compress|Confirm|Connect|Convert|ConvertFrom|ConvertTo|Copy|Debug|Deny|Disable|Disconnect|Dismount|Edit|Enable|Enter|Exit|Expand|Export|Find|ForEach|Format|Get|Grant|Group|Hide|Import|Initialize|Install|Invoke|Join|Limit|Lock|Measure|Merge|Move|New|Open|Optimize|Out|Ping|Pop|Protect|Publish|Push|Read|Receive|Redo|Register|Remove|Rename|Repair|Request|Reset|Resize|Resolve|Restart|Restore|Resume|Revoke|Save|Search|Select|Send|Set|Show|Skip|Sort|Split|Start|Step|Stop|Submit|Suspend|Switch|Sync|Tee|Test|Trace|Unblock|Undo|Uninstall|Unlock|Unprotect|Unpublish|Unregister|Update|Use|Wait|Watch|Where|Write)-[a-z]+\b/i, /\b(?:ac|cat|chdir|clc|cli|clp|clv|compare|copy|cp|cpi|cpp|cvpa|dbp|del|diff|dir|ebp|echo|epal|epcsv|epsn|erase|fc|fl|ft|fw|gal|gbp|gc|gci|gcs|gdr|gi|gl|gm|gp|gps|group|gsv|gu|gv|gwmi|iex|ii|ipal|ipcsv|ipsn|irm|iwmi|iwr|kill|lp|ls|measure|mi|mount|move|mp|mv|nal|ndr|ni|nv|ogv|popd|ps|pushd|pwd|rbp|rd|rdr|ren|ri|rm|rmdir|rni|rnp|rp|rv|rvpa|rwmi|sal|saps|sasv|sbp|sc|select|set|shcm|si|sl|sleep|sls|sort|sp|spps|spsv|start|sv|swmi|tee|trcm|type|write)\b/i], keyword: /\b(?:Begin|Break|Catch|Class|Continue|Data|Define|Do|DynamicParam|Else|ElseIf|End|Exit|Filter|Finally|For|ForEach|From|Function|If|InlineScript|Parallel|Param|Process|Return|Sequence|Switch|Throw|Trap|Try|Until|Using|Var|While|Workflow)\b/i, operator: { pattern: /(^|\W)(?:!|-(?:b?(?:and|x?or)|as|(?:Not)?(?:Contains|In|Like|Match)|eq|ge|gt|is(?:Not)?|Join|le|lt|ne|not|Replace|sh[lr])\b|-[-=]?|\+[+=]?|[*\/%]=?)/i, lookbehind: !0 }, punctuation: /[|{}[\];(),.]/ };i.string[0].inside = { "function": { pattern: /(^|[^`])\$\((?:\$\([^\r\n()]*\)|(?!\$\()[^\r\n)])*\)/, lookbehind: !0, inside: i }, boolean: i.boolean, variable: i.variable };
})(Prism);
Prism.languages.python = { comment: { pattern: /(^|[^\\])#.*/, lookbehind: !0, greedy: !0 }, "string-interpolation": { pattern: /(?:f|fr|rf)(?:("""|''')[\s\S]*?\1|("|')(?:\\.|(?!\2)[^\\\r\n])*\2)/i, greedy: !0, inside: { interpolation: { pattern: /((?:^|[^{])(?:\{\{)*)\{(?!\{)(?:[^{}]|\{(?!\{)(?:[^{}]|\{(?!\{)(?:[^{}])+\})+\})+\}/, lookbehind: !0, inside: { "format-spec": { pattern: /(:)[^:(){}]+(?=\}$)/, lookbehind: !0 }, "conversion-option": { pattern: /![sra](?=[:}]$)/, alias: "punctuation" }, rest: null } }, string: /[\s\S]+/ } }, "triple-quoted-string": { pattern: /(?:[rub]|br|rb)?("""|''')[\s\S]*?\1/i, greedy: !0, alias: "string" }, string: { pattern: /(?:[rub]|br|rb)?("|')(?:\\.|(?!\1)[^\\\r\n])*\1/i, greedy: !0 }, "function": { pattern: /((?:^|\s)def[ \t]+)[a-zA-Z_]\w*(?=\s*\()/g, lookbehind: !0 }, "class-name": { pattern: /(\bclass\s+)\w+/i, lookbehind: !0 }, decorator: { pattern: /(^[\t ]*)@\w+(?:\.\w+)*/m, lookbehind: !0, alias: ["annotation", "punctuation"], inside: { punctuation: /\./ } }, keyword: /\b(?:_(?=\s*:)|and|as|assert|async|await|break|case|class|continue|def|del|elif|else|except|exec|finally|for|from|global|if|import|in|is|lambda|match|nonlocal|not|or|pass|print|raise|return|try|while|with|yield)\b/, builtin: /\b(?:__import__|abs|all|any|apply|ascii|basestring|bin|bool|buffer|bytearray|bytes|callable|chr|classmethod|cmp|coerce|compile|complex|delattr|dict|dir|divmod|enumerate|eval|execfile|file|filter|float|format|frozenset|getattr|globals|hasattr|hash|help|hex|id|input|int|intern|isinstance|issubclass|iter|len|list|locals|long|map|max|memoryview|min|next|object|oct|open|ord|pow|property|range|raw_input|reduce|reload|repr|reversed|round|set|setattr|slice|sorted|staticmethod|str|sum|super|tuple|type|unichr|unicode|vars|xrange|zip)\b/, boolean: /\b(?:False|None|True)\b/, number: /\b0(?:b(?:_?[01])+|o(?:_?[0-7])+|x(?:_?[a-f0-9])+)\b|(?:\b\d+(?:_\d+)*(?:\.(?:\d+(?:_\d+)*)?)?|\B\.\d+(?:_\d+)*)(?:e[+-]?\d+(?:_\d+)*)?j?(?!\w)/i, operator: /[-+%=]=?|!=|:=|\*\*?=?|\/\/?=?|<[<=>]?|>[=>]?|[&|^~]/, punctuation: /[{}[\];(),.:]/ }, Prism.languages.python["string-interpolation"].inside.interpolation.inside.rest = Prism.languages.python, Prism.languages.py = Prism.languages.python;
!(function (e) {
  function s(e, s) {
    for (var a = 0; a < s; a++) e = e.replace(/<self>/g, function () {
      return "(?:" + e + ")";
    });return e.replace(/<self>/g, "[^\\s\\S]").replace(/<str>/g, '(?:@(?!")|"(?:[^\r\n\\\\"]|\\\\.)*"|@"(?:[^\\\\"]|""|\\\\[^])*"(?!")|\'(?:(?:[^\r\n\'\\\\]|\\\\.|\\\\[Uux][\\da-fA-F]{1,8})\'|(?=[^\\\\](?!\'))))').replace(/<comment>/g, "(?:/(?![/*])|//.*[\r\n]|/\\*[^*]*(?:\\*(?!/)[^*]*)*\\*/)");
  }var a = s("\\((?:[^()'\"@/]|<str>|<comment>|<self>)*\\)", 2),
      t = s("\\[(?:[^\\[\\]'\"@/]|<str>|<comment>|<self>)*\\]", 1),
      r = s("\\{(?:[^{}'\"@/]|<str>|<comment>|<self>)*\\}", 2),
      n = "@(?:await\\b\\s*)?(?:(?!await\\b)\\w+\\b|" + a + ")(?:[?!]?\\.\\w+\\b|(?:" + s("<(?:[^<>'\"@/]|<comment>|<self>)*>", 1) + ")?" + a + "|" + t + ")*(?![?!\\.(\\[]|<(?!/))",
      l = "(?:\"[^\"@]*\"|'[^'@]*'|[^\\s'\"@>=]+(?=[\\s>])|[\"'][^\"'@]*(?:(?:@(?![\\w()])|" + n + ")[^\"'@]*)+[\"'])",
      i = "(?:\\s(?:\\s*[^\\s>/=]+(?:\\s*=\\s*<tagAttrValue>|(?=[\\s/>])))+)?".replace(/<tagAttrValue>/, l),
      g = "(?!\\d)[^\\s>/=$<%]+" + i + "\\s*/?>",
      o = "\\B@?(?:<([a-zA-Z][\\w:]*)" + i + "\\s*>(?:[^<]|</?(?!\\1\\b)" + g + "|" + s("<\\1" + i + "\\s*>(?:[^<]|</?(?!\\1\\b)" + g + "|<self>)*</\\1\\s*>", 2) + ")*</\\1\\s*>|<" + g + ")";e.languages.cshtml = e.languages.extend("markup", {});var c = { pattern: /\S[\s\S]*/, alias: "language-csharp", inside: e.languages.insertBefore("csharp", "string", { html: { pattern: RegExp(o), greedy: !0, inside: e.languages.cshtml } }, { csharp: e.languages.extend("csharp", {}) }) },
      p = { pattern: RegExp("(^|[^@])" + n), lookbehind: !0, greedy: !0, alias: "variable", inside: { keyword: /^@/, csharp: c } };e.languages.cshtml.tag.pattern = RegExp("</?" + g), e.languages.cshtml.tag.inside["attr-value"].pattern = RegExp("=\\s*" + l), e.languages.insertBefore("inside", "punctuation", { value: p }, e.languages.cshtml.tag.inside["attr-value"]), e.languages.insertBefore("cshtml", "prolog", { "razor-comment": { pattern: /@\*[\s\S]*?\*@/, greedy: !0, alias: "comment" }, block: { pattern: RegExp("(^|[^@])@(?:" + [r, "(?:code|functions)\\s*" + r, "(?:for|foreach|lock|switch|using|while)\\s*" + a + "\\s*" + r, "do\\s*" + r + "\\s*while\\s*" + a + "(?:\\s*;)?", "try\\s*" + r + "\\s*catch\\s*" + a + "\\s*" + r + "\\s*finally\\s*" + r, "if\\s*" + a + "\\s*" + r + "(?:\\s*else(?:\\s+if\\s*" + a + ")?\\s*" + r + ")*", "helper\\s+\\w+\\s*" + a + "\\s*" + r].join("|") + ")"), lookbehind: !0, greedy: !0, inside: { keyword: /^@\w*/, csharp: c } }, directive: { pattern: /^([ \t]*)@(?:addTagHelper|attribute|implements|inherits|inject|layout|model|namespace|page|preservewhitespace|removeTagHelper|section|tagHelperPrefix|using)(?=\s).*/m, lookbehind: !0, greedy: !0, inside: { keyword: /^@\w+/, csharp: c } }, value: p, "delegate-operator": { pattern: /(^|[^@])@(?=<)/, lookbehind: !0, alias: "operator" } }), e.languages.razor = e.languages.cshtml;
})(Prism);
!(function (t) {
  var n = t.util.clone(t.languages.javascript),
      e = "(?:\\{<S>*\\.{3}(?:[^{}]|<BRACES>)*\\})";function a(t, n) {
    return t = t.replace(/<S>/g, function () {
      return "(?:\\s|//.*(?!.)|/\\*(?:[^*]|\\*(?!/))\\*/)";
    }).replace(/<BRACES>/g, function () {
      return "(?:\\{(?:\\{(?:\\{[^{}]*\\}|[^{}])*\\}|[^{}])*\\})";
    }).replace(/<SPREAD>/g, function () {
      return e;
    }), RegExp(t, n);
  }e = a(e).source, t.languages.jsx = t.languages.extend("markup", n), t.languages.jsx.tag.pattern = a("</?(?:[\\w.:-]+(?:<S>+(?:[\\w.:$-]+(?:=(?:\"(?:\\\\[^]|[^\\\\\"])*\"|'(?:\\\\[^]|[^\\\\'])*'|[^\\s{'\"/>=]+|<BRACES>))?|<SPREAD>))*<S>*/?)?>"), t.languages.jsx.tag.inside.tag.pattern = /^<\/?[^\s>\/]*/, t.languages.jsx.tag.inside["attr-value"].pattern = /=(?!\{)(?:"(?:\\[\s\S]|[^\\"])*"|'(?:\\[\s\S]|[^\\'])*'|[^\s'">]+)/, t.languages.jsx.tag.inside.tag.inside["class-name"] = /^[A-Z]\w*(?:\.[A-Z]\w*)*$/, t.languages.jsx.tag.inside.comment = n.comment, t.languages.insertBefore("inside", "attr-name", { spread: { pattern: a("<SPREAD>"), inside: t.languages.jsx } }, t.languages.jsx.tag), t.languages.insertBefore("inside", "special-attr", { script: { pattern: a("=<BRACES>"), alias: "language-javascript", inside: { "script-punctuation": { pattern: /^=(?=\{)/, alias: "punctuation" }, rest: t.languages.jsx } } }, t.languages.jsx.tag);var s = function s(t) {
    return t ? "string" == typeof t ? t : "string" == typeof t.content ? t.content : t.content.map(s).join("") : "";
  },
      g = function g(n) {
    for (var e = [], a = 0; a < n.length; a++) {
      var o = n[a],
          i = !1;if (("string" != typeof o && ("tag" === o.type && o.content[0] && "tag" === o.content[0].type ? "</" === o.content[0].content[0].content ? e.length > 0 && e[e.length - 1].tagName === s(o.content[0].content[1]) && e.pop() : "/>" === o.content[o.content.length - 1].content || e.push({ tagName: s(o.content[0].content[1]), openedBraces: 0 }) : e.length > 0 && "punctuation" === o.type && "{" === o.content ? e[e.length - 1].openedBraces++ : e.length > 0 && e[e.length - 1].openedBraces > 0 && "punctuation" === o.type && "}" === o.content ? e[e.length - 1].openedBraces-- : i = !0), (i || "string" == typeof o) && e.length > 0 && 0 === e[e.length - 1].openedBraces)) {
        var r = s(o);a < n.length - 1 && ("string" == typeof n[a + 1] || "plain-text" === n[a + 1].type) && (r += s(n[a + 1]), n.splice(a + 1, 1)), a > 0 && ("string" == typeof n[a - 1] || "plain-text" === n[a - 1].type) && (r = s(n[a - 1]) + r, n.splice(a - 1, 1), a--), n[a] = new t.Token("plain-text", r, null, r);
      }o.content && "string" != typeof o.content && g(o.content);
    }
  };t.hooks.add("after-tokenize", function (t) {
    "jsx" !== t.language && "tsx" !== t.language || g(t.tokens);
  });
})(Prism);
!(function (e) {
  e.languages.typescript = e.languages.extend("javascript", { "class-name": { pattern: /(\b(?:class|extends|implements|instanceof|interface|new|type)\s+)(?!keyof\b)(?!\s)[_$a-zA-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*(?:\s*<(?:[^<>]|<(?:[^<>]|<[^<>]*>)*>)*>)?/, lookbehind: !0, greedy: !0, inside: null }, builtin: /\b(?:Array|Function|Promise|any|boolean|console|never|number|string|symbol|unknown)\b/ }), e.languages.typescript.keyword.push(/\b(?:abstract|declare|is|keyof|readonly|require)\b/, /\b(?:asserts|infer|interface|module|namespace|type)\b(?=\s*(?:[{_$a-zA-Z\xA0-\uFFFF]|$))/, /\btype\b(?=\s*(?:[\{*]|$))/), delete e.languages.typescript.parameter, delete e.languages.typescript["literal-property"];var s = e.languages.extend("typescript", {});delete s["class-name"], e.languages.typescript["class-name"].inside = s, e.languages.insertBefore("typescript", "function", { decorator: { pattern: /@[$\w\xA0-\uFFFF]+/, inside: { at: { pattern: /^@/, alias: "operator" }, "function": /^[\s\S]+/ } }, "generic-function": { pattern: /#?(?!\s)[_$a-zA-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*\s*<(?:[^<>]|<(?:[^<>]|<[^<>]*>)*>)*>(?=\s*\()/, greedy: !0, inside: { "function": /^#?(?!\s)[_$a-zA-Z\xA0-\uFFFF](?:(?!\s)[$\w\xA0-\uFFFF])*/, generic: { pattern: /<[\s\S]+/, alias: "class-name", inside: s } } } }), e.languages.ts = e.languages.typescript;
})(Prism);
!(function (e) {
  var a = e.util.clone(e.languages.typescript);e.languages.tsx = e.languages.extend("jsx", a), delete e.languages.tsx.parameter, delete e.languages.tsx["literal-property"];var t = e.languages.tsx.tag;t.pattern = RegExp("(^|[^\\w$]|(?=</))(?:" + t.pattern.source + ")", t.pattern.flags), t.lookbehind = !0;
})(Prism);
!(function (a) {
  var e = { pattern: /\\[\\(){}[\]^$+*?|.]/, alias: "escape" },
      n = /\\(?:x[\da-fA-F]{2}|u[\da-fA-F]{4}|u\{[\da-fA-F]+\}|0[0-7]{0,2}|[123][0-7]{2}|c[a-zA-Z]|.)/,
      t = "(?:[^\\\\-]|" + n.source + ")",
      s = RegExp(t + "-" + t),
      i = { pattern: /(<|')[^<>']+(?=[>']$)/, lookbehind: !0, alias: "variable" };a.languages.regex = { "char-class": { pattern: /((?:^|[^\\])(?:\\\\)*)\[(?:[^\\\]]|\\[\s\S])*\]/, lookbehind: !0, inside: { "char-class-negation": { pattern: /(^\[)\^/, lookbehind: !0, alias: "operator" }, "char-class-punctuation": { pattern: /^\[|\]$/, alias: "punctuation" }, range: { pattern: s, inside: { escape: n, "range-punctuation": { pattern: /-/, alias: "operator" } } }, "special-escape": e, "char-set": { pattern: /\\[wsd]|\\p\{[^{}]+\}/i, alias: "class-name" }, escape: n } }, "special-escape": e, "char-set": { pattern: /\.|\\[wsd]|\\p\{[^{}]+\}/i, alias: "class-name" }, backreference: [{ pattern: /\\(?![123][0-7]{2})[1-9]/, alias: "keyword" }, { pattern: /\\k<[^<>']+>/, alias: "keyword", inside: { "group-name": i } }], anchor: { pattern: /[$^]|\\[ABbGZz]/, alias: "function" }, escape: n, group: [{ pattern: /\((?:\?(?:<[^<>']+>|'[^<>']+'|[>:]|<?[=!]|[idmnsuxU]+(?:-[idmnsuxU]+)?:?))?/, alias: "punctuation", inside: { "group-name": i } }, { pattern: /\)/, alias: "punctuation" }], quantifier: { pattern: /(?:[+*?]|\{\d+(?:,\d*)?\})[?+]?/, alias: "number" }, alternation: { pattern: /\|/, alias: "keyword" } };
})(Prism);
Prism.languages.rest = { table: [{ pattern: /(^[\t ]*)(?:\+[=-]+)+\+(?:\r?\n|\r)(?:\1[+|].+[+|](?:\r?\n|\r))+\1(?:\+[=-]+)+\+/m, lookbehind: !0, inside: { punctuation: /\||(?:\+[=-]+)+\+/ } }, { pattern: /(^[\t ]*)=+ [ =]*=(?:(?:\r?\n|\r)\1.+)+(?:\r?\n|\r)\1=+ [ =]*=(?=(?:\r?\n|\r){2}|\s*$)/m, lookbehind: !0, inside: { punctuation: /[=-]+/ } }], "substitution-def": { pattern: /(^[\t ]*\.\. )\|(?:[^|\s](?:[^|]*[^|\s])?)\| [^:]+::/m, lookbehind: !0, inside: { substitution: { pattern: /^\|(?:[^|\s]|[^|\s][^|]*[^|\s])\|/, alias: "attr-value", inside: { punctuation: /^\||\|$/ } }, directive: { pattern: /( )(?! )[^:]+::/, lookbehind: !0, alias: "function", inside: { punctuation: /::$/ } } } }, "link-target": [{ pattern: /(^[\t ]*\.\. )\[[^\]]+\]/m, lookbehind: !0, alias: "string", inside: { punctuation: /^\[|\]$/ } }, { pattern: /(^[\t ]*\.\. )_(?:`[^`]+`|(?:[^:\\]|\\.)+):/m, lookbehind: !0, alias: "string", inside: { punctuation: /^_|:$/ } }], directive: { pattern: /(^[\t ]*\.\. )[^:]+::/m, lookbehind: !0, alias: "function", inside: { punctuation: /::$/ } }, comment: { pattern: /(^[\t ]*\.\.)(?:(?: .+)?(?:(?:\r?\n|\r).+)+| .+)(?=(?:\r?\n|\r){2}|$)/m, lookbehind: !0 }, title: [{ pattern: /^(([!"#$%&'()*+,\-.\/:;<=>?@\[\\\]^_`{|}~])\2+)(?:\r?\n|\r).+(?:\r?\n|\r)\1$/m, inside: { punctuation: /^[!"#$%&'()*+,\-.\/:;<=>?@\[\\\]^_`{|}~]+|[!"#$%&'()*+,\-.\/:;<=>?@\[\\\]^_`{|}~]+$/, important: /.+/ } }, { pattern: /(^|(?:\r?\n|\r){2}).+(?:\r?\n|\r)([!"#$%&'()*+,\-.\/:;<=>?@\[\\\]^_`{|}~])\2+(?=\r?\n|\r|$)/, lookbehind: !0, inside: { punctuation: /[!"#$%&'()*+,\-.\/:;<=>?@\[\\\]^_`{|}~]+$/, important: /.+/ } }], hr: { pattern: /((?:\r?\n|\r){2})([!"#$%&'()*+,\-.\/:;<=>?@\[\\\]^_`{|}~])\2{3,}(?=(?:\r?\n|\r){2})/, lookbehind: !0, alias: "punctuation" }, field: { pattern: /(^[\t ]*):[^:\r\n]+:(?= )/m, lookbehind: !0, alias: "attr-name" }, "command-line-option": { pattern: /(^[\t ]*)(?:[+-][a-z\d]|(?:--|\/)[a-z\d-]+)(?:[ =](?:[a-z][\w-]*|<[^<>]+>))?(?:, (?:[+-][a-z\d]|(?:--|\/)[a-z\d-]+)(?:[ =](?:[a-z][\w-]*|<[^<>]+>))?)*(?=(?:\r?\n|\r)? {2,}\S)/im, lookbehind: !0, alias: "symbol" }, "literal-block": { pattern: /::(?:\r?\n|\r){2}([ \t]+)(?![ \t]).+(?:(?:\r?\n|\r)\1.+)*/, inside: { "literal-block-punctuation": { pattern: /^::/, alias: "punctuation" } } }, "quoted-literal-block": { pattern: /::(?:\r?\n|\r){2}([!"#$%&'()*+,\-.\/:;<=>?@\[\\\]^_`{|}~]).*(?:(?:\r?\n|\r)\1.*)*/, inside: { "literal-block-punctuation": { pattern: /^(?:::|([!"#$%&'()*+,\-.\/:;<=>?@\[\\\]^_`{|}~])\1*)/m, alias: "punctuation" } } }, "list-bullet": { pattern: /(^[\t ]*)(?:[*+\-•‣⁃]|\(?(?:\d+|[a-z]|[ivxdclm]+)\)|(?:\d+|[a-z]|[ivxdclm]+)\.)(?= )/im, lookbehind: !0, alias: "punctuation" }, "doctest-block": { pattern: /(^[\t ]*)>>> .+(?:(?:\r?\n|\r).+)*/m, lookbehind: !0, inside: { punctuation: /^>>>/ } }, inline: [{ pattern: /(^|[\s\-:\/'"<(\[{])(?::[^:]+:`.*?`|`.*?`:[^:]+:|(\*\*?|``?|\|)(?!\s)(?:(?!\2).)*\S\2(?=[\s\-.,:;!?\\\/'")\]}]|$))/m, lookbehind: !0, inside: { bold: { pattern: /(^\*\*).+(?=\*\*$)/, lookbehind: !0 }, italic: { pattern: /(^\*).+(?=\*$)/, lookbehind: !0 }, "inline-literal": { pattern: /(^``).+(?=``$)/, lookbehind: !0, alias: "symbol" }, role: { pattern: /^:[^:]+:|:[^:]+:$/, alias: "function", inside: { punctuation: /^:|:$/ } }, "interpreted-text": { pattern: /(^`).+(?=`$)/, lookbehind: !0, alias: "attr-value" }, substitution: { pattern: /(^\|).+(?=\|$)/, lookbehind: !0, alias: "attr-value" }, punctuation: /\*\*?|``?|\|/ } }], link: [{ pattern: /\[[^\[\]]+\]_(?=[\s\-.,:;!?\\\/'")\]}]|$)/, alias: "string", inside: { punctuation: /^\[|\]_$/ } }, { pattern: /(?:\b[a-z\d]+(?:[_.:+][a-z\d]+)*_?_|`[^`]+`_?_|_`[^`]+`)(?=[\s\-.,:;!?\\\/'")\]}]|$)/i, alias: "string", inside: { punctuation: /^_?`|`$|`?_?_$/ } }], punctuation: { pattern: /(^[\t ]*)(?:\|(?= |$)|(?:---?|—|\.\.|__)(?= )|\.\.$)/m, lookbehind: !0 } };
!(function (e) {
  e.languages.sass = e.languages.extend("css", { comment: { pattern: /^([ \t]*)\/[\/*].*(?:(?:\r?\n|\r)\1[ \t].+)*/m, lookbehind: !0, greedy: !0 } }), e.languages.insertBefore("sass", "atrule", { "atrule-line": { pattern: /^(?:[ \t]*)[@+=].+/m, greedy: !0, inside: { atrule: /(?:@[\w-]+|[+=])/ } } }), delete e.languages.sass.atrule;var r = /\$[-\w]+|#\{\$[-\w]+\}/,
      t = [/[+*\/%]|[=!]=|<=?|>=?|\b(?:and|not|or)\b/, { pattern: /(\s)-(?=\s)/, lookbehind: !0 }];e.languages.insertBefore("sass", "property", { "variable-line": { pattern: /^[ \t]*\$.+/m, greedy: !0, inside: { punctuation: /:/, variable: r, operator: t } }, "property-line": { pattern: /^[ \t]*(?:[^:\s]+ *:.*|:[^:\s].*)/m, greedy: !0, inside: { property: [/[^:\s]+(?=\s*:)/, { pattern: /(:)[^:\s]+/, lookbehind: !0 }], punctuation: /:/, variable: r, operator: t, important: e.languages.sass.important } } }), delete e.languages.sass.property, delete e.languages.sass.important, e.languages.insertBefore("sass", "punctuation", { selector: { pattern: /^([ \t]*)\S(?:,[^,\r\n]+|[^,\r\n]*)(?:,[^,\r\n]+)*(?:,(?:\r?\n|\r)\1[ \t]+\S(?:,[^,\r\n]+|[^,\r\n]*)(?:,[^,\r\n]+)*)*/m, lookbehind: !0, greedy: !0 } });
})(Prism);
Prism.languages.sql = { comment: { pattern: /(^|[^\\])(?:\/\*[\s\S]*?\*\/|(?:--|\/\/|#).*)/, lookbehind: !0 }, variable: [{ pattern: /@(["'`])(?:\\[\s\S]|(?!\1)[^\\])+\1/, greedy: !0 }, /@[\w.$]+/], string: { pattern: /(^|[^@\\])("|')(?:\\[\s\S]|(?!\2)[^\\]|\2\2)*\2/, greedy: !0, lookbehind: !0 }, identifier: { pattern: /(^|[^@\\])`(?:\\[\s\S]|[^`\\]|``)*`/, greedy: !0, lookbehind: !0, inside: { punctuation: /^`|`$/ } }, "function": /\b(?:AVG|COUNT|FIRST|FORMAT|LAST|LCASE|LEN|MAX|MID|MIN|MOD|NOW|ROUND|SUM|UCASE)(?=\s*\()/i, keyword: /\b(?:ACTION|ADD|AFTER|ALGORITHM|ALL|ALTER|ANALYZE|ANY|APPLY|AS|ASC|AUTHORIZATION|AUTO_INCREMENT|BACKUP|BDB|BEGIN|BERKELEYDB|BIGINT|BINARY|BIT|BLOB|BOOL|BOOLEAN|BREAK|BROWSE|BTREE|BULK|BY|CALL|CASCADED?|CASE|CHAIN|CHAR(?:ACTER|SET)?|CHECK(?:POINT)?|CLOSE|CLUSTERED|COALESCE|COLLATE|COLUMNS?|COMMENT|COMMIT(?:TED)?|COMPUTE|CONNECT|CONSISTENT|CONSTRAINT|CONTAINS(?:TABLE)?|CONTINUE|CONVERT|CREATE|CROSS|CURRENT(?:_DATE|_TIME|_TIMESTAMP|_USER)?|CURSOR|CYCLE|DATA(?:BASES?)?|DATE(?:TIME)?|DAY|DBCC|DEALLOCATE|DEC|DECIMAL|DECLARE|DEFAULT|DEFINER|DELAYED|DELETE|DELIMITERS?|DENY|DESC|DESCRIBE|DETERMINISTIC|DISABLE|DISCARD|DISK|DISTINCT|DISTINCTROW|DISTRIBUTED|DO|DOUBLE|DROP|DUMMY|DUMP(?:FILE)?|DUPLICATE|ELSE(?:IF)?|ENABLE|ENCLOSED|END|ENGINE|ENUM|ERRLVL|ERRORS|ESCAPED?|EXCEPT|EXEC(?:UTE)?|EXISTS|EXIT|EXPLAIN|EXTENDED|FETCH|FIELDS|FILE|FILLFACTOR|FIRST|FIXED|FLOAT|FOLLOWING|FOR(?: EACH ROW)?|FORCE|FOREIGN|FREETEXT(?:TABLE)?|FROM|FULL|FUNCTION|GEOMETRY(?:COLLECTION)?|GLOBAL|GOTO|GRANT|GROUP|HANDLER|HASH|HAVING|HOLDLOCK|HOUR|IDENTITY(?:COL|_INSERT)?|IF|IGNORE|IMPORT|INDEX|INFILE|INNER|INNODB|INOUT|INSERT|INT|INTEGER|INTERSECT|INTERVAL|INTO|INVOKER|ISOLATION|ITERATE|JOIN|KEYS?|KILL|LANGUAGE|LAST|LEAVE|LEFT|LEVEL|LIMIT|LINENO|LINES|LINESTRING|LOAD|LOCAL|LOCK|LONG(?:BLOB|TEXT)|LOOP|MATCH(?:ED)?|MEDIUM(?:BLOB|INT|TEXT)|MERGE|MIDDLEINT|MINUTE|MODE|MODIFIES|MODIFY|MONTH|MULTI(?:LINESTRING|POINT|POLYGON)|NATIONAL|NATURAL|NCHAR|NEXT|NO|NONCLUSTERED|NULLIF|NUMERIC|OFF?|OFFSETS?|ON|OPEN(?:DATASOURCE|QUERY|ROWSET)?|OPTIMIZE|OPTION(?:ALLY)?|ORDER|OUT(?:ER|FILE)?|OVER|PARTIAL|PARTITION|PERCENT|PIVOT|PLAN|POINT|POLYGON|PRECEDING|PRECISION|PREPARE|PREV|PRIMARY|PRINT|PRIVILEGES|PROC(?:EDURE)?|PUBLIC|PURGE|QUICK|RAISERROR|READS?|REAL|RECONFIGURE|REFERENCES|RELEASE|RENAME|REPEAT(?:ABLE)?|REPLACE|REPLICATION|REQUIRE|RESIGNAL|RESTORE|RESTRICT|RETURN(?:ING|S)?|REVOKE|RIGHT|ROLLBACK|ROUTINE|ROW(?:COUNT|GUIDCOL|S)?|RTREE|RULE|SAVE(?:POINT)?|SCHEMA|SECOND|SELECT|SERIAL(?:IZABLE)?|SESSION(?:_USER)?|SET(?:USER)?|SHARE|SHOW|SHUTDOWN|SIMPLE|SMALLINT|SNAPSHOT|SOME|SONAME|SQL|START(?:ING)?|STATISTICS|STATUS|STRIPED|SYSTEM_USER|TABLES?|TABLESPACE|TEMP(?:ORARY|TABLE)?|TERMINATED|TEXT(?:SIZE)?|THEN|TIME(?:STAMP)?|TINY(?:BLOB|INT|TEXT)|TOP?|TRAN(?:SACTIONS?)?|TRIGGER|TRUNCATE|TSEQUAL|TYPES?|UNBOUNDED|UNCOMMITTED|UNDEFINED|UNION|UNIQUE|UNLOCK|UNPIVOT|UNSIGNED|UPDATE(?:TEXT)?|USAGE|USE|USER|USING|VALUES?|VAR(?:BINARY|CHAR|CHARACTER|YING)|VIEW|WAITFOR|WARNINGS|WHEN|WHERE|WHILE|WITH(?: ROLLUP|IN)?|WORK|WRITE(?:TEXT)?|YEAR)\b/i, boolean: /\b(?:FALSE|NULL|TRUE)\b/i, number: /\b0x[\da-f]+\b|\b\d+(?:\.\d*)?|\B\.\d+\b/i, operator: /[-+*\/=%^~]|&&?|\|\|?|!=?|<(?:=>?|<|>)?|>[>=]?|\b(?:AND|BETWEEN|DIV|ILIKE|IN|IS|LIKE|NOT|OR|REGEXP|RLIKE|SOUNDS LIKE|XOR)\b/i, punctuation: /[;[\]()`,.]/ };
Prism.languages.iecst = { comment: [{ pattern: /(^|[^\\])(?:\/\*[\s\S]*?(?:\*\/|$)|\(\*[\s\S]*?(?:\*\)|$)|\{[\s\S]*?(?:\}|$))/, lookbehind: !0, greedy: !0 }, { pattern: /(^|[^\\:])\/\/.*/, lookbehind: !0, greedy: !0 }], string: { pattern: /(["'])(?:\\(?:\r\n|[\s\S])|(?!\1)[^\\\r\n])*\1/, greedy: !0 }, keyword: [/\b(?:END_)?(?:PROGRAM|CONFIGURATION|INTERFACE|FUNCTION_BLOCK|FUNCTION|ACTION|TRANSITION|TYPE|STRUCT|(?:INITIAL_)?STEP|NAMESPACE|LIBRARY|CHANNEL|FOLDER|RESOURCE|VAR_(?:ACCESS|CONFIG|EXTERNAL|GLOBAL|INPUT|IN_OUT|OUTPUT|TEMP)|VAR|METHOD|PROPERTY)\b/i, /\b(?:AT|BY|(?:END_)?(?:CASE|FOR|IF|REPEAT|WHILE)|CONSTANT|CONTINUE|DO|ELSE|ELSIF|EXIT|EXTENDS|FROM|GET|GOTO|IMPLEMENTS|JMP|NON_RETAIN|OF|PRIVATE|PROTECTED|PUBLIC|RETAIN|RETURN|SET|TASK|THEN|TO|UNTIL|USING|WITH|__CATCH|__ENDTRY|__FINALLY|__TRY)\b/], "class-name": /\b(?:ANY|ARRAY|BOOL|BYTE|U?(?:D|L|S)?INT|(?:D|L)?WORD|DATE(?:_AND_TIME)?|DT|L?REAL|POINTER|STRING|TIME(?:_OF_DAY)?|TOD)\b/, address: { pattern: /%[IQM][XBWDL][\d.]*|%[IQ][\d.]*/, alias: "symbol" }, number: /\b(?:16#[\da-f]+|2#[01_]+|0x[\da-f]+)\b|\b(?:D|DT|T|TOD)#[\d_shmd:]*|\b[A-Z]*#[\d.,_]*|(?:\b\d+(?:\.\d*)?|\B\.\d+)(?:e[+-]?\d+)?/i, boolean: /\b(?:FALSE|NULL|TRUE)\b/, operator: /S?R?:?=>?|&&?|\*\*?|<[=>]?|>=?|[-:^/+#]|\b(?:AND|EQ|EXPT|GE|GT|LE|LT|MOD|NE|NOT|OR|XOR)\b/, "function": /\b[a-z_]\w*(?=\s*\()/i, punctuation: /[()[\].,;]/ };
!(function (e) {
  function t(e, t, a) {
    return { pattern: RegExp("<#" + e + "[\\s\\S]*?#>"), alias: "block", inside: { delimiter: { pattern: RegExp("^<#" + e + "|#>$"), alias: "important" }, content: { pattern: /[\s\S]+/, inside: t, alias: a } } };
  }e.languages["t4-templating"] = Object.defineProperty({}, "createT4", { value: function value(a) {
      var n = e.languages[a],
          i = "language-" + a;return { block: { pattern: /<#[\s\S]+?#>/, inside: { directive: t("@", { "attr-value": { pattern: /=(?:("|')(?:\\[\s\S]|(?!\1)[^\\])*\1|[^\s'">=]+)/, inside: { punctuation: /^=|^["']|["']$/ } }, keyword: /\b\w+(?=\s)/, "attr-name": /\b\w+/ }), expression: t("=", n, i), "class-feature": t("\\+", n, i), standard: t("", n, i) } } };
    } });
})(Prism);
Prism.languages.t4 = Prism.languages["t4-cs"] = Prism.languages["t4-templating"].createT4("csharp");
Prism.languages.vim = { string: /"(?:[^"\\\r\n]|\\.)*"|'(?:[^'\r\n]|'')*'/, comment: /".*/, "function": /\b\w+(?=\()/, keyword: /\b(?:N|Next|P|Print|X|XMLent|XMLns|ab|abbreviate|abc|abclear|abo|aboveleft|al|all|ar|arga|argadd|argd|argdelete|argdo|arge|argedit|argg|argglobal|argl|arglocal|args|argu|argument|as|ascii|b|bN|bNext|ba|bad|badd|ball|bd|bdelete|be|bel|belowright|bf|bfirst|bl|blast|bm|bmodified|bn|bnext|bo|botright|bp|bprevious|br|brea|break|breaka|breakadd|breakd|breakdel|breakl|breaklist|brewind|bro|browse|bufdo|buffer|buffers|bun|bunload|bw|bwipeout|c|cN|cNext|cNfcNfile|ca|cabbrev|cabc|cabclear|cad|caddb|caddbuffer|caddexpr|caddf|caddfile|cal|call|cat|catch|cb|cbuffer|cc|ccl|cclose|cd|ce|center|cex|cexpr|cf|cfile|cfir|cfirst|cg|cgetb|cgetbuffer|cgete|cgetexpr|cgetfile|change|changes|chd|chdir|che|checkpath|checkt|checktime|cl|cla|clast|clist|clo|close|cmapc|cmapclear|cn|cnew|cnewer|cnext|cnf|cnfile|cnorea|cnoreabbrev|co|col|colder|colo|colorscheme|comc|comclear|comp|compiler|con|conf|confirm|continue|cope|copen|copy|cp|cpf|cpfile|cprevious|cq|cquit|cr|crewind|cu|cuna|cunabbrev|cunmap|cw|cwindow|d|debugg|debuggreedy|delc|delcommand|delete|delf|delfunction|delm|delmarks|di|diffg|diffget|diffoff|diffpatch|diffpu|diffput|diffsplit|diffthis|diffu|diffupdate|dig|digraphs|display|dj|djump|dl|dlist|dr|drop|ds|dsearch|dsp|dsplit|e|earlier|echoe|echoerr|echom|echomsg|echon|edit|el|else|elsei|elseif|em|emenu|en|endf|endfo|endfor|endfun|endfunction|endif|endt|endtry|endw|endwhile|ene|enew|ex|exi|exit|exu|exusage|f|file|files|filetype|fin|fina|finally|find|fini|finish|fir|first|fix|fixdel|fo|fold|foldc|foldclose|foldd|folddoc|folddoclosed|folddoopen|foldo|foldopen|for|fu|fun|function|go|goto|gr|grep|grepa|grepadd|h|ha|hardcopy|help|helpf|helpfind|helpg|helpgrep|helpt|helptags|hid|hide|his|history|ia|iabbrev|iabc|iabclear|if|ij|ijump|il|ilist|imapc|imapclear|in|inorea|inoreabbrev|isearch|isp|isplit|iu|iuna|iunabbrev|iunmap|j|join|ju|jumps|k|kee|keepalt|keepj|keepjumps|keepmarks|l|lN|lNext|lNf|lNfile|la|lad|laddb|laddbuffer|laddexpr|laddf|laddfile|lan|language|last|later|lb|lbuffer|lc|lcd|lch|lchdir|lcl|lclose|left|lefta|leftabove|let|lex|lexpr|lf|lfile|lfir|lfirst|lg|lgetb|lgetbuffer|lgete|lgetexpr|lgetfile|lgr|lgrep|lgrepa|lgrepadd|lh|lhelpgrep|list|ll|lla|llast|lli|llist|lm|lmak|lmake|lmap|lmapc|lmapclear|ln|lne|lnew|lnewer|lnext|lnf|lnfile|lnoremap|lo|loadview|loc|lockmarks|lockv|lockvar|lol|lolder|lop|lopen|lp|lpf|lpfile|lprevious|lr|lrewind|ls|lt|ltag|lu|lunmap|lv|lvimgrep|lvimgrepa|lvimgrepadd|lw|lwindow|m|ma|mak|make|mark|marks|mat|match|menut|menutranslate|mk|mkexrc|mks|mksession|mksp|mkspell|mkv|mkvie|mkview|mkvimrc|mod|mode|move|mz|mzf|mzfile|mzscheme|n|nbkey|new|next|nmapc|nmapclear|noh|nohlsearch|norea|noreabbrev|nu|number|nun|nunmap|o|omapc|omapclear|on|only|open|opt|options|ou|ounmap|p|pc|pclose|pe|ped|pedit|perl|perld|perldo|po|pop|popu|popup|pp|ppop|pre|preserve|prev|previous|print|prof|profd|profdel|profile|promptf|promptfind|promptr|promptrepl|ps|psearch|ptN|ptNext|pta|ptag|ptf|ptfirst|ptj|ptjump|ptl|ptlast|ptn|ptnext|ptp|ptprevious|ptr|ptrewind|pts|ptselect|pu|put|pw|pwd|py|pyf|pyfile|python|q|qa|qall|quit|quita|quitall|r|read|rec|recover|red|redi|redir|redo|redr|redraw|redraws|redrawstatus|reg|registers|res|resize|ret|retab|retu|return|rew|rewind|ri|right|rightb|rightbelow|ru|rub|ruby|rubyd|rubydo|rubyf|rubyfile|runtime|rv|rviminfo|sN|sNext|sa|sal|sall|san|sandbox|sargument|sav|saveas|sb|sbN|sbNext|sba|sball|sbf|sbfirst|sbl|sblast|sbm|sbmodified|sbn|sbnext|sbp|sbprevious|sbr|sbrewind|sbuffer|scrip|scripte|scriptencoding|scriptnames|se|set|setf|setfiletype|setg|setglobal|setl|setlocal|sf|sfind|sfir|sfirst|sh|shell|sign|sil|silent|sim|simalt|sl|sla|slast|sleep|sm|smagic|smap|smapc|smapclear|sme|smenu|sn|snext|sni|sniff|sno|snomagic|snor|snoremap|snoreme|snoremenu|so|sor|sort|source|sp|spe|spelld|spelldump|spellgood|spelli|spellinfo|spellr|spellrepall|spellu|spellundo|spellw|spellwrong|split|spr|sprevious|sre|srewind|st|sta|stag|star|startg|startgreplace|startinsert|startr|startreplace|stj|stjump|stop|stopi|stopinsert|sts|stselect|sun|sunhide|sunm|sunmap|sus|suspend|sv|sview|syncbind|t|tN|tNext|ta|tab|tabN|tabNext|tabc|tabclose|tabd|tabdo|tabe|tabedit|tabf|tabfind|tabfir|tabfirst|tabl|tablast|tabm|tabmove|tabn|tabnew|tabnext|tabo|tabonly|tabp|tabprevious|tabr|tabrewind|tabs|tag|tags|tc|tcl|tcld|tcldo|tclf|tclfile|te|tearoff|tf|tfirst|th|throw|tj|tjump|tl|tlast|tm|tmenu|tn|tnext|to|topleft|tp|tprevious|tr|trewind|try|ts|tselect|tu|tunmenu|u|una|unabbreviate|undo|undoj|undojoin|undol|undolist|unh|unhide|unlet|unlo|unlockvar|unm|unmap|up|update|ve|verb|verbose|version|vert|vertical|vi|vie|view|vim|vimgrep|vimgrepa|vimgrepadd|visual|viu|viusage|vmapc|vmapclear|vne|vnew|vs|vsplit|vu|vunmap|w|wN|wNext|wa|wall|wh|while|win|winc|wincmd|windo|winp|winpos|winsize|wn|wnext|wp|wprevious|wq|wqa|wqall|write|ws|wsverb|wv|wviminfo|x|xa|xall|xit|xm|xmap|xmapc|xmapclear|xme|xmenu|xn|xnoremap|xnoreme|xnoremenu|xu|xunmap|y|yank)\b/, builtin: /\b(?:acd|ai|akm|aleph|allowrevins|altkeymap|ambiwidth|ambw|anti|antialias|arab|arabic|arabicshape|ari|arshape|autochdir|autocmd|autoindent|autoread|autowrite|autowriteall|aw|awa|background|backspace|backup|backupcopy|backupdir|backupext|backupskip|balloondelay|ballooneval|balloonexpr|bdir|bdlay|beval|bex|bexpr|bg|bh|bin|binary|biosk|bioskey|bk|bkc|bomb|breakat|brk|browsedir|bs|bsdir|bsk|bt|bufhidden|buflisted|buftype|casemap|ccv|cdpath|cedit|cfu|ch|charconvert|ci|cin|cindent|cink|cinkeys|cino|cinoptions|cinw|cinwords|clipboard|cmdheight|cmdwinheight|cmp|cms|columns|com|comments|commentstring|compatible|complete|completefunc|completeopt|consk|conskey|copyindent|cot|cpo|cpoptions|cpt|cscopepathcomp|cscopeprg|cscopequickfix|cscopetag|cscopetagorder|cscopeverbose|cspc|csprg|csqf|cst|csto|csverb|cuc|cul|cursorcolumn|cursorline|cwh|debug|deco|def|define|delcombine|dex|dg|dict|dictionary|diff|diffexpr|diffopt|digraph|dip|dir|directory|dy|ea|ead|eadirection|eb|ed|edcompatible|ef|efm|ei|ek|enc|encoding|endofline|eol|ep|equalalways|equalprg|errorbells|errorfile|errorformat|esckeys|et|eventignore|expandtab|exrc|fcl|fcs|fdc|fde|fdi|fdl|fdls|fdm|fdn|fdo|fdt|fen|fenc|fencs|fex|ff|ffs|fileencoding|fileencodings|fileformat|fileformats|fillchars|fk|fkmap|flp|fml|fmr|foldcolumn|foldenable|foldexpr|foldignore|foldlevel|foldlevelstart|foldmarker|foldmethod|foldminlines|foldnestmax|foldtext|formatexpr|formatlistpat|formatoptions|formatprg|fp|fs|fsync|ft|gcr|gd|gdefault|gfm|gfn|gfs|gfw|ghr|gp|grepformat|grepprg|gtl|gtt|guicursor|guifont|guifontset|guifontwide|guiheadroom|guioptions|guipty|guitablabel|guitabtooltip|helpfile|helpheight|helplang|hf|hh|hi|hidden|highlight|hk|hkmap|hkmapp|hkp|hl|hlg|hls|hlsearch|ic|icon|iconstring|ignorecase|im|imactivatekey|imak|imc|imcmdline|imd|imdisable|imi|iminsert|ims|imsearch|inc|include|includeexpr|incsearch|inde|indentexpr|indentkeys|indk|inex|inf|infercase|insertmode|invacd|invai|invakm|invallowrevins|invaltkeymap|invanti|invantialias|invar|invarab|invarabic|invarabicshape|invari|invarshape|invautochdir|invautoindent|invautoread|invautowrite|invautowriteall|invaw|invawa|invbackup|invballooneval|invbeval|invbin|invbinary|invbiosk|invbioskey|invbk|invbl|invbomb|invbuflisted|invcf|invci|invcin|invcindent|invcompatible|invconfirm|invconsk|invconskey|invcopyindent|invcp|invcscopetag|invcscopeverbose|invcst|invcsverb|invcuc|invcul|invcursorcolumn|invcursorline|invdeco|invdelcombine|invdg|invdiff|invdigraph|invdisable|invea|inveb|inved|invedcompatible|invek|invendofline|inveol|invequalalways|inverrorbells|invesckeys|invet|invex|invexpandtab|invexrc|invfen|invfk|invfkmap|invfoldenable|invgd|invgdefault|invguipty|invhid|invhidden|invhk|invhkmap|invhkmapp|invhkp|invhls|invhlsearch|invic|invicon|invignorecase|invim|invimc|invimcmdline|invimd|invincsearch|invinf|invinfercase|invinsertmode|invis|invjoinspaces|invjs|invlazyredraw|invlbr|invlinebreak|invlisp|invlist|invloadplugins|invlpl|invlz|invma|invmacatsui|invmagic|invmh|invml|invmod|invmodeline|invmodifiable|invmodified|invmore|invmousef|invmousefocus|invmousehide|invnu|invnumber|invodev|invopendevice|invpaste|invpi|invpreserveindent|invpreviewwindow|invprompt|invpvw|invreadonly|invremap|invrestorescreen|invrevins|invri|invrightleft|invrightleftcmd|invrl|invrlc|invro|invrs|invru|invruler|invsb|invsc|invscb|invscrollbind|invscs|invsecure|invsft|invshellslash|invshelltemp|invshiftround|invshortname|invshowcmd|invshowfulltag|invshowmatch|invshowmode|invsi|invsm|invsmartcase|invsmartindent|invsmarttab|invsmd|invsn|invsol|invspell|invsplitbelow|invsplitright|invspr|invsr|invssl|invsta|invstartofline|invstmp|invswapfile|invswf|invta|invtagbsearch|invtagrelative|invtagstack|invtbi|invtbidi|invtbs|invtermbidi|invterse|invtextauto|invtextmode|invtf|invtgst|invtildeop|invtimeout|invtitle|invto|invtop|invtr|invttimeout|invttybuiltin|invttyfast|invtx|invvb|invvisualbell|invwa|invwarn|invwb|invweirdinvert|invwfh|invwfw|invwildmenu|invwinfixheight|invwinfixwidth|invwiv|invwmnu|invwrap|invwrapscan|invwrite|invwriteany|invwritebackup|invws|isf|isfname|isi|isident|isk|iskeyword|isprint|joinspaces|js|key|keymap|keymodel|keywordprg|km|kmp|kp|langmap|langmenu|laststatus|lazyredraw|lbr|lcs|linebreak|lines|linespace|lisp|lispwords|listchars|loadplugins|lpl|lsp|lz|macatsui|magic|makeef|makeprg|matchpairs|matchtime|maxcombine|maxfuncdepth|maxmapdepth|maxmem|maxmempattern|maxmemtot|mco|mef|menuitems|mfd|mh|mis|mkspellmem|ml|mls|mm|mmd|mmp|mmt|modeline|modelines|modifiable|modified|more|mouse|mousef|mousefocus|mousehide|mousem|mousemodel|mouses|mouseshape|mouset|mousetime|mp|mps|msm|mzq|mzquantum|nf|noacd|noai|noakm|noallowrevins|noaltkeymap|noanti|noantialias|noar|noarab|noarabic|noarabicshape|noari|noarshape|noautochdir|noautoindent|noautoread|noautowrite|noautowriteall|noaw|noawa|nobackup|noballooneval|nobeval|nobin|nobinary|nobiosk|nobioskey|nobk|nobl|nobomb|nobuflisted|nocf|noci|nocin|nocindent|nocompatible|noconfirm|noconsk|noconskey|nocopyindent|nocp|nocscopetag|nocscopeverbose|nocst|nocsverb|nocuc|nocul|nocursorcolumn|nocursorline|nodeco|nodelcombine|nodg|nodiff|nodigraph|nodisable|noea|noeb|noed|noedcompatible|noek|noendofline|noeol|noequalalways|noerrorbells|noesckeys|noet|noex|noexpandtab|noexrc|nofen|nofk|nofkmap|nofoldenable|nogd|nogdefault|noguipty|nohid|nohidden|nohk|nohkmap|nohkmapp|nohkp|nohls|noic|noicon|noignorecase|noim|noimc|noimcmdline|noimd|noincsearch|noinf|noinfercase|noinsertmode|nois|nojoinspaces|nojs|nolazyredraw|nolbr|nolinebreak|nolisp|nolist|noloadplugins|nolpl|nolz|noma|nomacatsui|nomagic|nomh|noml|nomod|nomodeline|nomodifiable|nomodified|nomore|nomousef|nomousefocus|nomousehide|nonu|nonumber|noodev|noopendevice|nopaste|nopi|nopreserveindent|nopreviewwindow|noprompt|nopvw|noreadonly|noremap|norestorescreen|norevins|nori|norightleft|norightleftcmd|norl|norlc|noro|nors|noru|noruler|nosb|nosc|noscb|noscrollbind|noscs|nosecure|nosft|noshellslash|noshelltemp|noshiftround|noshortname|noshowcmd|noshowfulltag|noshowmatch|noshowmode|nosi|nosm|nosmartcase|nosmartindent|nosmarttab|nosmd|nosn|nosol|nospell|nosplitbelow|nosplitright|nospr|nosr|nossl|nosta|nostartofline|nostmp|noswapfile|noswf|nota|notagbsearch|notagrelative|notagstack|notbi|notbidi|notbs|notermbidi|noterse|notextauto|notextmode|notf|notgst|notildeop|notimeout|notitle|noto|notop|notr|nottimeout|nottybuiltin|nottyfast|notx|novb|novisualbell|nowa|nowarn|nowb|noweirdinvert|nowfh|nowfw|nowildmenu|nowinfixheight|nowinfixwidth|nowiv|nowmnu|nowrap|nowrapscan|nowrite|nowriteany|nowritebackup|nows|nrformats|numberwidth|nuw|odev|oft|ofu|omnifunc|opendevice|operatorfunc|opfunc|osfiletype|pa|para|paragraphs|paste|pastetoggle|patchexpr|patchmode|path|pdev|penc|pex|pexpr|pfn|ph|pheader|pi|pm|pmbcs|pmbfn|popt|preserveindent|previewheight|previewwindow|printdevice|printencoding|printexpr|printfont|printheader|printmbcharset|printmbfont|printoptions|prompt|pt|pumheight|pvh|pvw|qe|quoteescape|readonly|remap|report|restorescreen|revins|rightleft|rightleftcmd|rl|rlc|ro|rs|rtp|ruf|ruler|rulerformat|runtimepath|sbo|sc|scb|scr|scroll|scrollbind|scrolljump|scrolloff|scrollopt|scs|sect|sections|secure|sel|selection|selectmode|sessionoptions|sft|shcf|shellcmdflag|shellpipe|shellquote|shellredir|shellslash|shelltemp|shelltype|shellxquote|shiftround|shiftwidth|shm|shortmess|shortname|showbreak|showcmd|showfulltag|showmatch|showmode|showtabline|shq|si|sidescroll|sidescrolloff|siso|sj|slm|smartcase|smartindent|smarttab|smc|smd|softtabstop|sol|spc|spell|spellcapcheck|spellfile|spelllang|spellsuggest|spf|spl|splitbelow|splitright|sps|sr|srr|ss|ssl|ssop|stal|startofline|statusline|stl|stmp|su|sua|suffixes|suffixesadd|sw|swapfile|swapsync|swb|swf|switchbuf|sws|sxq|syn|synmaxcol|syntax|t_AB|t_AF|t_AL|t_CS|t_CV|t_Ce|t_Co|t_Cs|t_DL|t_EI|t_F1|t_F2|t_F3|t_F4|t_F5|t_F6|t_F7|t_F8|t_F9|t_IE|t_IS|t_K1|t_K3|t_K4|t_K5|t_K6|t_K7|t_K8|t_K9|t_KA|t_KB|t_KC|t_KD|t_KE|t_KF|t_KG|t_KH|t_KI|t_KJ|t_KK|t_KL|t_RI|t_RV|t_SI|t_Sb|t_Sf|t_WP|t_WS|t_ZH|t_ZR|t_al|t_bc|t_cd|t_ce|t_cl|t_cm|t_cs|t_da|t_db|t_dl|t_fs|t_k1|t_k2|t_k3|t_k4|t_k5|t_k6|t_k7|t_k8|t_k9|t_kB|t_kD|t_kI|t_kN|t_kP|t_kb|t_kd|t_ke|t_kh|t_kl|t_kr|t_ks|t_ku|t_le|t_mb|t_md|t_me|t_mr|t_ms|t_nd|t_op|t_se|t_so|t_sr|t_te|t_ti|t_ts|t_ue|t_us|t_ut|t_vb|t_ve|t_vi|t_vs|t_xs|tabline|tabpagemax|tabstop|tagbsearch|taglength|tagrelative|tagstack|tal|tb|tbi|tbidi|tbis|tbs|tenc|term|termbidi|termencoding|terse|textauto|textmode|textwidth|tgst|thesaurus|tildeop|timeout|timeoutlen|title|titlelen|titleold|titlestring|toolbar|toolbariconsize|top|tpm|tsl|tsr|ttimeout|ttimeoutlen|ttm|tty|ttybuiltin|ttyfast|ttym|ttymouse|ttyscroll|ttytype|tw|tx|uc|ul|undolevels|updatecount|updatetime|ut|vb|vbs|vdir|verbosefile|vfile|viewdir|viewoptions|viminfo|virtualedit|visualbell|vop|wak|warn|wb|wc|wcm|wd|weirdinvert|wfh|wfw|whichwrap|wi|wig|wildchar|wildcharm|wildignore|wildmenu|wildmode|wildoptions|wim|winaltkeys|window|winfixheight|winfixwidth|winheight|winminheight|winminwidth|winwidth|wiv|wiw|wm|wmh|wmnu|wmw|wop|wrap|wrapmargin|wrapscan|writeany|writebackup|writedelay|ww)\b/, number: /\b(?:0x[\da-f]+|\d+(?:\.\d+)?)\b/i, operator: /\|\||&&|[-+.]=?|[=!](?:[=~][#?]?)?|[<>]=?[#?]?|[*\/%?]|\b(?:is(?:not)?)\b/, punctuation: /[{}[\](),;:]/ };
!(function (e) {
  var n = /[*&][^\s[\]{},]+/,
      r = /!(?:<[\w\-%#;/?:@&=+$,.!~*'()[\]]+>|(?:[a-zA-Z\d-]*!)?[\w\-%#;/?:@&=+$.~*'()]+)?/,
      t = "(?:" + r.source + "(?:[ \t]+" + n.source + ")?|" + n.source + "(?:[ \t]+" + r.source + ")?)",
      a = "(?:[^\\s\\x00-\\x08\\x0e-\\x1f!\"#%&'*,\\-:>?@[\\]`{|}\\x7f-\\x84\\x86-\\x9f\\ud800-\\udfff\\ufffe\\uffff]|[?:-]<PLAIN>)(?:[ \t]*(?:(?![#:])<PLAIN>|:<PLAIN>))*".replace(/<PLAIN>/g, function () {
    return "[^\\s\\x00-\\x08\\x0e-\\x1f,[\\]{}\\x7f-\\x84\\x86-\\x9f\\ud800-\\udfff\\ufffe\\uffff]";
  }),
      d = "\"(?:[^\"\\\\\r\n]|\\\\.)*\"|'(?:[^'\\\\\r\n]|\\\\.)*'";function o(e, n) {
    n = (n || "").replace(/m/g, "") + "m";var r = "([:\\-,[{]\\s*(?:\\s<<prop>>[ \t]+)?)(?:<<value>>)(?=[ \t]*(?:$|,|\\]|\\}|(?:[\r\n]\\s*)?#))".replace(/<<prop>>/g, function () {
      return t;
    }).replace(/<<value>>/g, function () {
      return e;
    });return RegExp(r, n);
  }e.languages.yaml = { scalar: { pattern: RegExp("([\\-:]\\s*(?:\\s<<prop>>[ \t]+)?[|>])[ \t]*(?:((?:\r?\n|\r)[ \t]+)\\S[^\r\n]*(?:\\2[^\r\n]+)*)".replace(/<<prop>>/g, function () {
        return t;
      })), lookbehind: !0, alias: "string" }, comment: /#.*/, key: { pattern: RegExp("((?:^|[:\\-,[{\r\n?])[ \t]*(?:<<prop>>[ \t]+)?)<<key>>(?=\\s*:\\s)".replace(/<<prop>>/g, function () {
        return t;
      }).replace(/<<key>>/g, function () {
        return "(?:" + a + "|" + d + ")";
      })), lookbehind: !0, greedy: !0, alias: "atrule" }, directive: { pattern: /(^[ \t]*)%.+/m, lookbehind: !0, alias: "important" }, datetime: { pattern: o("\\d{4}-\\d\\d?-\\d\\d?(?:[tT]|[ \t]+)\\d\\d?:\\d{2}:\\d{2}(?:\\.\\d*)?(?:[ \t]*(?:Z|[-+]\\d\\d?(?::\\d{2})?))?|\\d{4}-\\d{2}-\\d{2}|\\d\\d?:\\d{2}(?::\\d{2}(?:\\.\\d*)?)?"), lookbehind: !0, alias: "number" }, boolean: { pattern: o("false|true", "i"), lookbehind: !0, alias: "important" }, "null": { pattern: o("null|~", "i"), lookbehind: !0, alias: "important" }, string: { pattern: o(d), lookbehind: !0, greedy: !0 }, number: { pattern: o("[+-]?(?:0x[\\da-f]+|0o[0-7]+|(?:\\d+(?:\\.\\d*)?|\\.\\d+)(?:e[+-]?\\d+)?|\\.inf|\\.nan)", "i"), lookbehind: !0 }, tag: r, important: n, punctuation: /---|[:[\]{}\-,|>?]|\.\.\./ }, e.languages.yml = e.languages.yaml;
})(Prism);
!(function () {
  if ("undefined" != typeof Prism && "undefined" != typeof document && document.querySelector) {
    var e,
        t = "line-numbers",
        i = "linkable-line-numbers",
        n = /\n(?!$)/g,
        r = !0;Prism.plugins.lineHighlight = { highlightLines: function highlightLines(o, u, c) {
        var h = (u = "string" == typeof u ? u : o.getAttribute("data-line") || "").replace(/\s+/g, "").split(",").filter(Boolean),
            d = +o.getAttribute("data-line-offset") || 0,
            f = ((function () {
          if (void 0 === e) {
            var t = document.createElement("div");t.style.fontSize = "13px", t.style.lineHeight = "1.5", t.style.padding = "0", t.style.border = "0", t.innerHTML = "&nbsp;<br />&nbsp;", document.body.appendChild(t), e = 38 === t.offsetHeight, document.body.removeChild(t);
          }return e;
        })() ? parseInt : parseFloat)(getComputedStyle(o).lineHeight),
            p = Prism.util.isActive(o, t),
            g = o.querySelector("code"),
            m = p ? o : g || o,
            v = [],
            y = g.textContent.match(n),
            b = y ? y.length + 1 : 1,
            A = g && m != g ? (function (e, t) {
          var i = getComputedStyle(e),
              n = getComputedStyle(t);function r(e) {
            return +e.substr(0, e.length - 2);
          }return t.offsetTop + r(n.borderTopWidth) + r(n.paddingTop) - r(i.paddingTop);
        })(o, g) : 0;h.forEach(function (e) {
          var t = e.split("-"),
              i = +t[0],
              n = +t[1] || i;if (!((n = Math.min(b + d, n)) < i)) {
            var r = o.querySelector('.line-highlight[data-range="' + e + '"]') || document.createElement("div");if ((v.push(function () {
              r.setAttribute("aria-hidden", "true"), r.setAttribute("data-range", e), r.className = (c || "") + " line-highlight";
            }), p && Prism.plugins.lineNumbers)) {
              var s = Prism.plugins.lineNumbers.getLine(o, i),
                  l = Prism.plugins.lineNumbers.getLine(o, n);if (s) {
                var a = s.offsetTop + A + "px";v.push(function () {
                  r.style.top = a;
                });
              }if (l) {
                var u = l.offsetTop - s.offsetTop + l.offsetHeight + "px";v.push(function () {
                  r.style.height = u;
                });
              }
            } else v.push(function () {
              r.setAttribute("data-start", String(i)), n > i && r.setAttribute("data-end", String(n)), r.style.top = (i - d - 1) * f + A + "px", r.textContent = new Array(n - i + 2).join(" \n");
            });v.push(function () {
              r.style.width = o.scrollWidth + "px";
            }), v.push(function () {
              m.appendChild(r);
            });
          }
        });var P = o.id;if (p && Prism.util.isActive(o, i) && P) {
          l(o, i) || v.push(function () {
            o.classList.add(i);
          });var E = parseInt(o.getAttribute("data-start") || "1");s(".line-numbers-rows > span", o).forEach(function (e, t) {
            var i = t + E;e.onclick = function () {
              var e = P + "." + i;r = !1, location.hash = e, setTimeout(function () {
                r = !0;
              }, 1);
            };
          });
        }return function () {
          v.forEach(a);
        };
      } };var o = 0;Prism.hooks.add("before-sanity-check", function (e) {
      var t = e.element.parentElement;if (u(t)) {
        var i = 0;s(".line-highlight", t).forEach(function (e) {
          i += e.textContent.length, e.parentNode.removeChild(e);
        }), i && /^(?: \n)+$/.test(e.code.slice(-i)) && (e.code = e.code.slice(0, -i));
      }
    }), Prism.hooks.add("complete", function e(i) {
      var n = i.element.parentElement;if (u(n)) {
        clearTimeout(o);var r = Prism.plugins.lineNumbers,
            s = i.plugins && i.plugins.lineNumbers;l(n, t) && r && !s ? Prism.hooks.add("line-numbers", e) : (Prism.plugins.lineHighlight.highlightLines(n)(), o = setTimeout(c, 1));
      }
    }), window.addEventListener("hashchange", c), window.addEventListener("resize", function () {
      s("pre").filter(u).map(function (e) {
        return Prism.plugins.lineHighlight.highlightLines(e);
      }).forEach(a);
    });
  }function s(e, t) {
    return Array.prototype.slice.call((t || document).querySelectorAll(e));
  }function l(e, t) {
    return e.classList.contains(t);
  }function a(e) {
    e();
  }function u(e) {
    return !!(e && /pre/i.test(e.nodeName) && (e.hasAttribute("data-line") || e.id && Prism.util.isActive(e, i)));
  }function c() {
    var e = location.hash.slice(1);s(".temporary.line-highlight").forEach(function (e) {
      e.parentNode.removeChild(e);
    });var t = (e.match(/\.([\d,-]+)$/) || [, ""])[1];if (t && !document.getElementById(e)) {
      var i = e.slice(0, e.lastIndexOf(".")),
          n = document.getElementById(i);n && (n.hasAttribute("data-line") || n.setAttribute("data-line", ""), Prism.plugins.lineHighlight.highlightLines(n, t, "temporary ")(), r && document.querySelector(".temporary.line-highlight").scrollIntoView());
    }
  }
})();
!(function () {
  if ("undefined" != typeof Prism && "undefined" != typeof document) {
    var e = "line-numbers",
        n = /\n(?!$)/g,
        t = Prism.plugins.lineNumbers = { getLine: function getLine(n, t) {
        if ("PRE" === n.tagName && n.classList.contains(e)) {
          var i = n.querySelector(".line-numbers-rows");if (i) {
            var r = parseInt(n.getAttribute("data-start"), 10) || 1,
                s = r + (i.children.length - 1);t < r && (t = r), t > s && (t = s);var l = t - r;return i.children[l];
          }
        }
      }, resize: function resize(e) {
        r([e]);
      }, assumeViewportIndependence: !0 },
        i = void 0;window.addEventListener("resize", function () {
      t.assumeViewportIndependence && i === window.innerWidth || (i = window.innerWidth, r(Array.prototype.slice.call(document.querySelectorAll("pre.line-numbers"))));
    }), Prism.hooks.add("complete", function (t) {
      if (t.code) {
        var i = t.element,
            s = i.parentNode;if (s && /pre/i.test(s.nodeName) && !i.querySelector(".line-numbers-rows") && Prism.util.isActive(i, e)) {
          i.classList.remove(e), s.classList.add(e);var l,
              o = t.code.match(n),
              a = o ? o.length + 1 : 1,
              u = new Array(a + 1).join("<span></span>");(l = document.createElement("span")).setAttribute("aria-hidden", "true"), l.className = "line-numbers-rows", l.innerHTML = u, s.hasAttribute("data-start") && (s.style.counterReset = "linenumber " + (parseInt(s.getAttribute("data-start"), 10) - 1)), t.element.appendChild(l), r([s]), Prism.hooks.run("line-numbers", t);
        }
      }
    }), Prism.hooks.add("line-numbers", function (e) {
      e.plugins = e.plugins || {}, e.plugins.lineNumbers = !0;
    });
  }function r(e) {
    if (0 != (e = e.filter(function (e) {
      var n,
          t = (n = e, n ? window.getComputedStyle ? getComputedStyle(n) : n.currentStyle || null : null)["white-space"];return "pre-wrap" === t || "pre-line" === t;
    })).length) {
      var t = e.map(function (e) {
        var t = e.querySelector("code"),
            i = e.querySelector(".line-numbers-rows");if (t && i) {
          var r = e.querySelector(".line-numbers-sizer"),
              s = t.textContent.split(n);r || ((r = document.createElement("span")).className = "line-numbers-sizer", t.appendChild(r)), r.innerHTML = "0", r.style.display = "block";var l = r.getBoundingClientRect().height;return r.innerHTML = "", { element: e, lines: s, lineHeights: [], oneLinerHeight: l, sizer: r };
        }
      }).filter(Boolean);t.forEach(function (e) {
        var n = e.sizer,
            t = e.lines,
            i = e.lineHeights,
            r = e.oneLinerHeight;i[t.length - 1] = void 0, t.forEach(function (e, t) {
          if (e && e.length > 1) {
            var s = n.appendChild(document.createElement("span"));s.style.display = "block", s.textContent = e;
          } else i[t] = r;
        });
      }), t.forEach(function (e) {
        for (var n = e.sizer, t = e.lineHeights, i = 0, r = 0; r < t.length; r++) void 0 === t[r] && (t[r] = n.children[i++].getBoundingClientRect().height);
      }), t.forEach(function (e) {
        var n = e.sizer,
            t = e.element.querySelector(".line-numbers-rows");n.style.display = "none", n.innerHTML = "", e.lineHeights.forEach(function (e, n) {
          t.children[n].style.height = e + "px";
        });
      });
    }
  }
})();
!(function () {
  if ("undefined" != typeof Prism) {
    var i = /\b([a-z]{3,7}:\/\/|tel:)[\w\-+%~/.:=&!$'()*,;@]+(?:\?[\w\-+%~/.:=?&!$'()*,;@]*)?(?:#[\w\-+%~/.:#=?&!$'()*,;@]*)?/,
        n = /\b\S+@[\w.]+[a-z]{2}/,
        t = /\[([^\]]+)\]\(([^)]+)\)/,
        e = ["comment", "url", "attr-value", "string"];Prism.plugins.autolinker = { processGrammar: function processGrammar(r) {
        r && !r["url-link"] && (Prism.languages.DFS(r, function (r, a, l) {
          e.indexOf(l) > -1 && !Array.isArray(a) && (a.pattern || (a = this[r] = { pattern: a }), a.inside = a.inside || {}, "comment" == l && (a.inside["md-link"] = t), "attr-value" == l ? Prism.languages.insertBefore("inside", "punctuation", { "url-link": i }, a) : a.inside["url-link"] = i, a.inside["email-link"] = n);
        }), r["url-link"] = i, r["email-link"] = n);
      } }, Prism.hooks.add("before-highlight", function (i) {
      Prism.plugins.autolinker.processGrammar(i.grammar);
    }), Prism.hooks.add("wrap", function (i) {
      if (/-link$/.test(i.type)) {
        i.tag = "a";var n = i.content;if ("email-link" == i.type && 0 != n.indexOf("mailto:")) n = "mailto:" + n;else if ("md-link" == i.type) {
          var e = i.content.match(t);n = e[2], i.content = e[1];
        }i.attributes.href = n;try {
          i.content = decodeURIComponent(i.content);
        } catch (i) {}
      }
    });
  }
})();
!(function () {
  if ("undefined" != typeof Prism && "undefined" != typeof document) {
    var e = [],
        t = {},
        n = function n() {};Prism.plugins.toolbar = {};var a = Prism.plugins.toolbar.registerButton = function (n, a) {
      var r;r = "function" == typeof a ? a : function (e) {
        var t;return "function" == typeof a.onClick ? ((t = document.createElement("button")).type = "button", t.addEventListener("click", function () {
          a.onClick.call(this, e);
        })) : "string" == typeof a.url ? (t = document.createElement("a")).href = a.url : t = document.createElement("span"), a.className && t.classList.add(a.className), t.textContent = a.text, t;
      }, n in t ? console.warn('There is a button with the key "' + n + '" registered already.') : e.push(t[n] = r);
    },
        r = Prism.plugins.toolbar.hook = function (a) {
      var r = a.element.parentNode;if (r && /pre/i.test(r.nodeName) && !r.parentNode.classList.contains("code-toolbar")) {
        var o = document.createElement("div");o.classList.add("code-toolbar"), r.parentNode.insertBefore(o, r), o.appendChild(r);var i = document.createElement("div");i.classList.add("toolbar");var l = e,
            d = (function (e) {
          for (; e;) {
            var t = e.getAttribute("data-toolbar-order");if (null != t) return (t = t.trim()).length ? t.split(/\s*,\s*/g) : [];e = e.parentElement;
          }
        })(a.element);d && (l = d.map(function (e) {
          return t[e] || n;
        })), l.forEach(function (e) {
          var t = e(a);if (t) {
            var n = document.createElement("div");n.classList.add("toolbar-item"), n.appendChild(t), i.appendChild(n);
          }
        }), o.appendChild(i);
      }
    };a("label", function (e) {
      var t = e.element.parentNode;if (t && /pre/i.test(t.nodeName) && t.hasAttribute("data-label")) {
        var n,
            a,
            r = t.getAttribute("data-label");try {
          a = document.querySelector("template#" + r);
        } catch (e) {}return a ? n = a.content : (t.hasAttribute("data-url") ? (n = document.createElement("a")).href = t.getAttribute("data-url") : n = document.createElement("span"), n.textContent = r), n;
      }
    }), Prism.hooks.add("complete", r);
  }
})();
!(function () {
  function t(t) {
    var e = document.createElement("textarea");e.value = t.getText(), e.style.top = "0", e.style.left = "0", e.style.position = "fixed", document.body.appendChild(e), e.focus(), e.select();try {
      var o = document.execCommand("copy");setTimeout(function () {
        o ? t.success() : t.error();
      }, 1);
    } catch (e) {
      setTimeout(function () {
        t.error(e);
      }, 1);
    }document.body.removeChild(e);
  }"undefined" != typeof Prism && "undefined" != typeof document && (Prism.plugins.toolbar ? Prism.plugins.toolbar.registerButton("copy-to-clipboard", function (e) {
    var o = e.element,
        n = (function (t) {
      var e = { copy: "Copy", "copy-error": "Press Ctrl+C to copy", "copy-success": "Copied!", "copy-timeout": 5e3 };for (var o in e) {
        for (var n = "data-prismjs-" + o, c = t; c && !c.hasAttribute(n);) c = c.parentElement;c && (e[o] = c.getAttribute(n));
      }return e;
    })(o),
        c = document.createElement("button");c.className = "copy-to-clipboard-button", c.setAttribute("type", "button");var r = document.createElement("span");return c.appendChild(r), u("copy"), (function (e, o) {
      e.addEventListener("click", function () {
        !(function (e) {
          navigator.clipboard ? navigator.clipboard.writeText(e.getText()).then(e.success, function () {
            t(e);
          }) : t(e);
        })(o);
      });
    })(c, { getText: function getText() {
        return o.textContent;
      }, success: function success() {
        u("copy-success"), i();
      }, error: function error() {
        u("copy-error"), setTimeout(function () {
          !(function (t) {
            window.getSelection().selectAllChildren(t);
          })(o);
        }, 1), i();
      } }), c;function i() {
      setTimeout(function () {
        u("copy");
      }, n["copy-timeout"]);
    }function u(t) {
      r.textContent = n[t], c.setAttribute("data-copy-state", t);
    }
  }) : console.warn("Copy to Clipboard plugin loaded before Toolbar plugin."));
})();

