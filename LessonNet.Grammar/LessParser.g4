/*
 [The "MIT licence"]
 Copyright (c) 2014 Kyle Lee
 All rights reserved.
*/

parser grammar LessParser;

options { tokenVocab=LessLexer; }

stylesheet
  : statement*
  ;

statement
  : importDeclaration
  | ruleset
  | variableDeclaration ';'
  | mixinDefinition
  | mediaBlock
  | mixinCall ';'
  ;

variableName
  : AT variableName
  | AT Identifier
  ;

mathCharacter
  : TIMES | PLUS | DIV | MINUS | PERC
  ;

parenthesizedExpression
    : LPAREN expression RPAREN
    ;

expressionList
  : expression+
  ;

expression
  : quotedExpression
  | parenthesizedExpression
  | expression mathCharacter expression
  | measurementList
  | measurement
  | function
  | Color
  | StringLiteral
  | url
  | variableName
  | identifier
  ;

quotedExpression
  : TIL StringLiteral
  ;

functionName
  : identifier
  | PERC
  ;

function
  : functionName LPAREN valueList? RPAREN
  ;

conditions
  : condition ((AND|COMMA) condition)*
  ;

condition
  : LPAREN conditionStatement RPAREN
  | NOT LPAREN conditionStatement RPAREN
  ;

conditionStatement
  : expression ( EQ | LT | GT | GTEQ | LTEQ ) expression
  | expression
  ;

variableDeclaration
  : variableName COLON valueList
  ;

/* Imports */
importDeclaration
  : '@import' referenceUrl importMediaTypes? ';'
  ;

referenceUrl
    : StringLiteral
    | UrlStart Url UrlEnd
    ;

importMediaTypes
  : (Identifier (COMMA Identifier)*)
  ;

/* Rules */
ruleset
   : selectors block
  ;

block
  : BlockStart (property ';' | statement)* property? BlockEnd
  ;

mixinDefinition
  : selectors LPAREN (mixinDefinitionParam ((';'|',') mixinDefinitionParam)*)? Ellipsis? RPAREN mixinGuard? block
  ;

mixinCall
  : selectors LPAREN (commaSeparatedExpressionList (';' commaSeparatedExpressionList)*)? RPAREN
  ;

mixinGuard
  : WHEN conditions
  ;

mixinDefinitionParam
  : variableName
  | variableDeclaration
  ;

selectors
  : selector (COMMA selector)*
  ;

selector
  : selectorElement+
  ;

attrib
  : LBRACK Identifier (attribRelate (StringLiteral | Identifier))? RBRACK
  ;

parentSelectorReference
    : PARENTREF 
    ;

combinator
  : (GT | PLUS | TIL | TIMES)
  ;

selectorElement
  : parentSelectorReference
  | ( HASH identifier
    | DOT identifier 
    | (COLON|COLONCOLON) Identifier
    | attrib
    | identifier
	| combinator )
  ;

attribRelate
  : '='
  | '~='
  | '|='
  ;

identifier
  : Identifier identifierPart*
  | InterpolationStart identifierVariableName BlockEnd identifierPart*
  ;

identifierPart
  : InterpolationStartAfter identifierVariableName BlockEnd
  | IdentifierAfter
  ;

identifierVariableName
  : (Identifier | IdentifierAfter)
  ;

property
  : identifier COLON valueList IMPORTANT?
  ;

valueList
  : commaSeparatedExpressionList
  | expressionList
  ;

commaSeparatedExpressionList
  : expressionList (COMMA expressionList)*
  ;

measurementList
  : measurement measurement+
  ;

url
  : UrlStart Url UrlEnd
  ;

measurement
  : Number Unit?
  ;

featureQuery
  : LPAREN property RPAREN
  | identifier
  ;

mediaQuery
  : MediaQueryModifier? featureQuery (AND featureQuery)*
  ;

mediaBlock
  : MEDIA mediaQuery (COMMA mediaQuery)* block
  ;
