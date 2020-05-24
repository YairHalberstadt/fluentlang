Write-Output "Generating FluentLangParser"

java -jar (Join-Path -Path $PSScriptRoot -ChildPath antlr-4.7.2-complete.jar) `
    -lib (Join-Path -Path $PSScriptRoot -ChildPath ../grammar) `
    -o (Join-Path -Path $PSScriptRoot -ChildPath ../source/Compiler/Generated) `
    -listener `
    -visitor `
    -Dlanguage=CSharp `
    -package FluentLang.Compiler.Generated `
    (Join-Path -Path $PSScriptRoot -ChildPath ../grammar/FluentLangLexer.g4) `
    (Join-Path -Path $PSScriptRoot -ChildPath ../grammar/FluentLangParser.g4)