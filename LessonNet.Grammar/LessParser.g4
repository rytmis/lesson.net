/*
 [The "MIT licence"]
 Copyright (c) 2014 Kyle Lee
 All rights reserved.
*/

parser grammar LessParser;

options { tokenVocab=LessLexer; }

stylesheet
  : terminatedStatement* statement?
  ;

statement
  : lineStatement
  | blockStatement
  ;

terminatedStatement
  : lineStatement SEMI
  | blockStatement SEMI?
  ;

blockStatement
  : mediaBlock
  | atRule
  | ruleset
  | mixinDefinition
  ;

lineStatement
  : importDeclaration
  | variableDeclaration
  | PARENTREF extend
  | property
  | mixinCall
  ;

atRule
  : toplevelAtRule
  | nestedAtRule
  ;

toplevelAtRule
  : charsetAtRule
  | namespaceAtRule
  ;

nestedAtRule
  : supportsAtRule
  | documentAtRule
  | pageAtRule
  | keyframesAtRule
  | genericAtRule
  ;

charsetAtRule : CHARSET string;
namespaceAtRule : NAMESPACE identifier? (string|url);

supportsAtRule : SUPPORTS supportsDeclaration block;

supportsDeclaration
  : supportsCondition
  | supportsConditionList
  ;

supportsCondition
  : NOT LPAREN (property|supportsConditionList) RPAREN
  | LPAREN property RPAREN
  ;

supportsConditionList
  : supportsCondition (AND supportsCondition)*
  | supportsCondition (OR supportsCondition)*
  ;

documentAtRule : DOCUMENT documentSpecifierList block;

documentSpecifierList
  : documentSpecifier (COMMA documentSpecifier)*;

documentSpecifier
  : url
  | function
  ;

pageAtRule
  : PAGE selector? block;

keyframesAtRule
  : AT KEYFRAMES identifier keyframesBlock;

keyframesBlock
  : BlockStart keyframe* BlockEnd;

keyframe
  : singleValuedExpression (COMMA singleValuedExpression)* block
  ;

genericAtRule
  : AT identifier expression? block?
  ;

variableName
  : AT variableName
  | AT identifier
  | ARGUMENTS
  | REST
  | MEDIA
  | IMPORT
  | CHARSET
  | NAMESPACE
  | SUPPORTS
  | DOCUMENT
  | PAGE
  | KEYFRAMES
  | COUNTERSTYLE
  | ARGUMENTS
  | REST
  ;


parenthesizedExpression
    : LPAREN expression RPAREN
    ;

color
  : HexColor
  | KnownColor
  ;

expression
  : { matchCommaSeparatedLists }? singleValuedExpression+ (COMMA singleValuedExpression+)+
  | singleValuedExpression+
  ;

singleValuedExpression
  : quotedExpression
  | escapeSequence
  | parenthesizedExpression
  | singleValuedExpression op=(DIV|TIMES) singleValuedExpression
  | singleValuedExpression op=(PLUS|MINUS) singleValuedExpression
  | measurement
  | color
  | function
  | string
  | url
  | variableName
  | booleanValue
  | unicodeRange
  | identifier
  | selector
  ;

unicodeRange
  : UnicodeValue (MINUS UnicodeValue)?
  ;

ieFilter
  : IE_FILTER_PROGID ieFilterIdentifier LPAREN (ieFilterExpression (COMMA ieFilterExpression)*) RPAREN
  ;

ieFilterIdentifier
  : identifier (DOT identifier)*
  ;

ieFilterExpression
  : identifier EQ singleValuedExpression
  ;

booleanValue : (TRUE | FALSE);

string
  : SQUOT_STRING_START (variableInterpolation | SQUOT_STRING_FRAGMENT)* SQUOT_STRING_END
  | DQUOT_STRING_START (variableInterpolation | DQUOT_STRING_FRAGMENT)* DQUOT_STRING_END
  ;

quotedExpression
  : TIL string
  ;

escapeSequence
  : EscapeSequence
  ;

functionName
  : identifier
  | PERC
  ;

function
  : { EnterCommaMode(true); } functionName LPAREN  expression? RPAREN { ExitCommaMode(); }
  ;

guardConditions
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
  : variableName COLON expression+ IMPORTANT?
  ;

/* Imports */
importDeclaration
  : IMPORT (LPAREN importOption (COMMA importOption)* RPAREN)? referenceUrl  (mediaQuery (COMMA mediaQuery)*)?
  | IMPORT_ONCE referenceUrl (mediaQuery (COMMA mediaQuery)*)?
  ;

importOption
  : (REFERENCE | INLINE | LESS | CSS | ONCE | MULTIPLE | OPTIONAL)
  ;


referenceUrl
    : string
    | quotedExpression
    | url
    ;

/* Rules */
ruleset
   : selectors rulesetGuard? block
  ;

rulesetGuard
  : WHEN guardConditions
  ;

block
  : BlockStart terminatedStatement* statement? BlockEnd
  ;

mixinDefinition
  : mixinDeclaration mixinGuard? block
  ;

mixinDeclaration
  : selector LPAREN (mixinDefinitionParam SEMI) RPAREN
  | selector LPAREN (mixinDefinitionParam (SEMI mixinDefinitionParam)+)? RPAREN 
  | 
    { EnterCommaMode(false); } 
      selector LPAREN (mixinDefinitionParam (COMMA mixinDefinitionParam)*)? RPAREN
    { ExitCommaMode(); }
  ;

mixinCallArgument
  : variableDeclaration
  | expression
  ;

mixinCall
  : selector LPAREN mixinCallArgument SEMI RPAREN IMPORTANT?
  | selector LPAREN mixinCallArgument (SEMI mixinCallArgument)+ RPAREN IMPORTANT?
  | 
    { matchCommaSeparatedLists = false; } 
      selector (LPAREN (mixinCallArgument (COMMA mixinCallArgument)*)? RPAREN)? IMPORTANT? 
    { matchCommaSeparatedLists = true; }
  ;



mixinGuard
  : WHEN mixinGuardDefault
  | WHEN guardConditions
  ;

mixinGuardDefault
  : LPAREN DEFAULT_FN LPAREN RPAREN RPAREN;

mixinDefinitionParam
  : variableName Ellipsis?
  | variableDeclaration
  | identifier
  | string
  | Number
  | Ellipsis
  ;

selectors
  : selectorListElement (COMMA selectorListElement)*
  ;

selectorListElement
  : selector extend?
  ;

selector
  : selectorElement+
  ;

attrib
  : LBRACK identifier (attribRelate attribValue)? RBRACK
  ;

attribValue
  : string 
  | identifier
  | measurement
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
    | pseudoClass
    | attrib
    | identifier
    | combinator )
  ;

extend
  : COLON EXTEND LPAREN extenderList RPAREN
  ;

extenderList
  : extender (COMMA extender)*;

extender
  : selector ALL?
  ;


pseudoClass
  : (COLON|COLONCOLON) pseudoclassIdentifier (LPAREN expression RPAREN)?;

attribRelate
  : EQ 
  | TILD_EQ
  | PIPE_EQ
  | CIRC_EQ
  | DOLLAR_EQ
  | STAR_EQ
  ;

keywordAsIdentifier
  : keywordAsPseudoclassIdentifier
  | EXTEND
  ;

keywordAsPseudoclassIdentifier
  : NULL
  | IN
  | URL
  | NOT
  | ONLY
  | REFERENCE
  | INLINE
  | LESS
  | CSS
  | ONCE
  | MULTIPLE
  | OPTIONAL
  | WHEN
  | NOT
  | AND
  | COLOR
  | CONVERT
  | DATA_URI
  | DEFAULT_FN
  | UNIT
  | GET_UNIT
  | SVG_GRADIENT
  | ESCAPE
  | E_FN
  | REPLACE
  | LENGTH
  | EXTRACT
  | CEIL
  | FLOOR
  | PERCENTAGE
  | ROUND
  | SQRT
  | ABS
  | SIN
  | ASIN
  | COS
  | ACOS
  | TAN
  | ATAN
  | PI
  | POW
  | MOD
  | MIN
  | MAX
  | ISNUMBER
  | ISSTRING
  | ISCOLOR
  | ISKEYWORD
  | ISURL
  | ISPIXEL
  | ISEM
  | ISPERCENTAGE
  | ISUNIT
  | RGB
  | RGBA
  | ARGB
  | HSL
  | HSLA
  | HSV
  | HSVA
  | HUE
  | SATURATION
  | LIGHTNESS
  | HSVHUE
  | HSVSATURATION
  | HSVVALUE
  | RED
  | GREEN
  | BLUE
  | ALPHA
  | LUMA
  | LUMINANCE
  | SATURATE
  | DESATURATE
  | LIGHTEN
  | DARKEN
  | FADEIN
  | FADEOUT
  | FADE
  | SPIN
  | MIX
  | GREYSCALE
  | CONTRAST
  | MULTIPLY
  | SCREEN
  | OVERLAY
  | SOFTLIGHT
  | HARDLIGHT
  | DIFFERENCE
  | EXCLUSION
  | AVERAGE
  | NEGATION
  | Unit
  | KnownColor
  | ALL
  | FROM
  | TO
  | TRUE
  | FALSE
  ;

variableInterpolation
  : InterpolationStart identifierVariableName BlockEnd 
  ;

identifier
  : (keywordAsIdentifier | Identifier | IdentifierAfter) identifierPart*
  | variableInterpolation identifierPart*
  ;

pseudoclassIdentifier
  : (keywordAsPseudoclassIdentifier | Identifier | IdentifierAfter) identifierPart*
  | variableInterpolation identifierPart*
  ;

identifierPart
  : InterpolationStartAfter identifierVariableName BlockEnd
  | IdentifierAfter
  ;

identifierVariableName
  : (Identifier | IdentifierAfter)
  ;

property
  : identifier COLON (expression|ieFilter)? IMPORTANT?
  ;

url
  : UrlStart (string|quotedExpression|variableName|rawUrl)? RPAREN
  ;

rawUrl
  : TIL? Url
  ;

unit
  : (PERC | Unit)
  ;

measurement
  : Number (NUMBER_SPACE|unit|identifier)?
  ;

mediaQueryModifier
  : (NOT|ONLY)
  ;

featureQuery
  : mediaQueryModifier? LPAREN property RPAREN
  | mediaQueryModifier? LPAREN (identifier|variableName) RPAREN
  | mediaQueryModifier? (identifier|variableName)
  ;

mediaQuery
  : featureQuery (AND featureQuery)*
  ;


mediaBlock
  : MEDIA mediaQuery (COMMA mediaQuery)* block
  ;
