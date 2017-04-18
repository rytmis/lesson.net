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
  | mixinDefinition
  | variableDeclaration ';'
  | mediaBlock
  | mixinCall ';'
  ;

variableName
  : AT variableName
  | AT Identifier
  | AT URL
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

color
  : HexColor
  | KnownColor
  ;

expression
  : quotedExpression
  | parenthesizedExpression
  | fraction
  | expression mathCharacter expression
  | measurementList
  | measurement
  | color
  | function
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

mixinGuardConditions
  : conditionList (COMMA conditionList)*
  ;

conditionList
  : condition (AND condition)*
  ;

condition
  : LPAREN conditionStatement RPAREN
  | NOT LPAREN conditionStatement RPAREN
  ;

comparisonOperator
  : ( EQ | LT | GT | GTEQ | LTEQ_MIXINGUARD )
  ;

comparison
  : expression comparisonOperator expression
  ;

conditionStatement
  : comparison
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

mixinCallArgument
  : variableDeclaration
  | valueList
  ;

mixinCall
  : selectors (LPAREN (mixinCallArgument (SEMI mixinCallArgument)*)? RPAREN)?
  | selectors (LPAREN (mixinCallArgument (COMMA mixinCallArgument)*)? RPAREN)?
  ;

mixinGuard
  : WHEN mixinGuardConditions
  ;

mixinDefinitionParam
  : variableName
  | variableDeclaration
  | identifier
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
  | HexColor
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
  : (KnownColor | Identifier) identifierPart*
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
  : expressionList (COMMA expressionList)*?
  ;

measurementList
  : measurement measurement+
  ;

url
  : UrlStart Url UrlEnd
  ;

fraction
  : Number DIV Number Unit?
  ;

measurement
  : Number Unit?
  ;

featureQuery
  : MediaQueryModifier? LPAREN property RPAREN
  | MediaQueryModifier? (identifier|variableName)
  ;

mediaQuery
  : featureQuery (AND featureQuery)*
  ;

mediaBlock
  : MEDIA mediaQuery (COMMA mediaQuery)* block
  ;
