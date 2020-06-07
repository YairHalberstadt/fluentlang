ace.define('ace/mode/fluentlang-mode',
    [
        "require",
        "exports",
        "module",
        "ace/lib/oop",
        "ace/mode/text",
        "ace/mode/text_highlight_rules",
        "ace/worker/worker_client"
    ],
    function (require, exports, module) {
        var oop = require("ace/lib/oop");
        var TextMode = require("ace/mode/text").Mode;
        var TextHighlightRules = require("ace/mode/text_highlight_rules").TextHighlightRules;

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
                    { token: "string", regex: /'(?:.|\\(:?u[\da-fA-F]+|x[\da-fA-F]+|[tbrf'"n]))?'/ },
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
        };
        oop.inherits(FluentLangMode, TextMode);

        (function () {

            this.$id = "ace/mode/fluentlang-mode";

        }).call(FluentLangMode.prototype);

        exports.Mode = FluentLangMode;
    });