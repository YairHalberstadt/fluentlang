ace.define("ace/mode/matching_brace_outdent", ["require", "exports", "module", "ace/range"], function (require, exports, module) {
    "use strict";

    var Range = require("ace/range").Range;

    var MatchingBraceOutdent = function () { };

    (function () {

        this.checkOutdent = function (line, input) {
            if (! /^\s+$/.test(line))
                return false;

            return /^\s*\}/.test(input);
        };

        this.autoOutdent = function (doc, row) {
            var line = doc.getLine(row);
            var match = line.match(/^(\s*\})/);

            if (!match) return 0;

            var column = match[1].length;
            var openBracePos = doc.findMatchingBracket({ row: row, column: column });

            if (!openBracePos || openBracePos.row == row) return 0;

            var indent = this.$getIndent(doc.getLine(openBracePos.row));
            doc.replace(new Range(row, 0, row, column - 1), indent);
        };

        this.$getIndent = function (line) {
            return line.match(/^\s*/)[0];
        };

    }).call(MatchingBraceOutdent.prototype);

    exports.MatchingBraceOutdent = MatchingBraceOutdent;
});

ace.define('ace/mode/fluentlang-mode',
    [
        "require",
        "exports",
        "module",
        "ace/lib/oop",
        "ace/mode/text",
        "ace/mode/text_highlight_rules",
        "ace/worker/worker_client",
        "ace/mode/matching_brace_outdent",
    ],
    function (require, exports, module) {
        "use strict";
        var oop = require("ace/lib/oop");
        var TextMode = require("ace/mode/text").Mode;
        var TextHighlightRules = require("ace/mode/text_highlight_rules").TextHighlightRules;
        var MatchingBraceOutdent = require("ace/mode/matching_brace_outdent").MatchingBraceOutdent;

        var FluentLangHighlightRules = function () {
            var keywordMapper = this.createKeywordMapper({
                "keyword.control": "if|else|return|match|let",
                "keyword.operator": "mixin",
                "keyword.other": "namespace|open",
                "storage.type": "interface|bool|int|double|char|string",
                "storage.modifier": "export|public",
                "constant.language": "true|false"
            }, "identifier");
            this.$rules = {
                "start": [
                    { token: "comment", regex: "\\/\\/.*$" },
                    {
                        token: "string", // character
                        regex: /'(?:.|\\(:?u[\da-fA-F]+|x[\da-fA-F]+|[tbrf'"n]))?'/
                    },
                    {
                        token: "string", start: '"', end: '"|$', next: [
                            { token: "constant.language.escape", regex: /\\(:?u[\da-fA-F]+|x[\da-fA-F]+|[tbrf'"n])/ },
                            { token: "invalid", regex: /\\./ }
                        ]
                    },
                    { token: "constant.numeric", regex: "[0-9]*\\.[0-9]+" },
                    { token: "constant.numeric", regex: "[0-9]+" },
                    { token: "keyword.operator", regex: "\\+|-|\\*|/|%|<|>|==|!=|<=|>=|&&|\\||=|\\.\\." },
                    { token: "punctuation.operator", regex: "\\:|\\,|\\;|\\.|\\_|=>" },
                    { token: "paren.lparen", regex: "[({]" },
                    { token: "paren.rparen", regex: "[)}]" },
                    { token: "text", regex: "\\s+" },
                    { token: keywordMapper, regex: "[a-zA-Z_$][a-zA-Z0-9_$]*\\b" }
                ]
            };
        };
        oop.inherits(FluentLangHighlightRules, TextHighlightRules);

        var FluentLangMode = function () {
            this.HighlightRules = FluentLangHighlightRules;
            this.$outdent = new MatchingBraceOutdent();
        };
        oop.inherits(FluentLangMode, TextMode);

        (function () {

            this.getNextLineIndent = function (state, line, tab) {
                var indent = this.$getIndent(line);

                if (state == "start") {
                    var match = line.match(/^.*[\{\(\[]\s*$/);
                    if (match) {
                        indent += tab;
                    }
                }

                return indent;
            };

            this.checkOutdent = function (state, line, input) {
                return this.$outdent.checkOutdent(line, input);
            };

            this.autoOutdent = function (state, doc, row) {
                this.$outdent.autoOutdent(doc, row);
            };

            this.$id = "ace/mode/fluentlang-mode";

        }).call(FluentLangMode.prototype);

        exports.Mode = FluentLangMode;
    });