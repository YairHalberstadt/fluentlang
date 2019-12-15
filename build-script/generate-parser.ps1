Write-Output "Generating FluentLangParser"

java -jar .\antlr-4.7.2-complete.jar `
    -lib ../grammar `
    -o ../source/Compiler/Generated `
    -listener `
    -visitor `
    -Dlanguage=CSharp `
    -package FluentLang.Compiler.Generated `
    ../grammar/FluentLangLexer.g4 `
    ../grammar/FluentLangParser.g4