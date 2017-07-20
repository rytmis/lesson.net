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
  | mediaBlock
  | variableDeclaration SEMI
  | atRule
  | ruleset
  | mixinDefinition
  | PARENTREF extend SEMI
  | mixinCall SEMI
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
  : KEYFRAMES identifier keyframesBlock;

keyframesBlock
  : BlockStart keyframe* BlockEnd;

keyframe
  : (FROM | TO | (Number PERC)) block;

genericAtRule
  : AT identifier valueList? block?
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
  | string
  | url
  | variableName
  | identifier
  | selector
  ;



string
  : SQUOT_STRING_START (variableInterpolation | SQUOT_STRING_FRAGMENT)* SQUOT_STRING_END
  | DQUOT_STRING_START (variableInterpolation | DQUOT_STRING_FRAGMENT)* DQUOT_STRING_END
  ;

quotedExpression
  : TIL string
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
  : variableName COLON valueList IMPORTANT?
  ;

/* Imports */
importDeclaration
  : '@import' referenceUrl importMediaTypes? ';'
  ;

referenceUrl
    : StringLiteral
    | UrlStart Url RPAREN
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
  : selector LPAREN (mixinDefinitionParam ((';'|',') mixinDefinitionParam)*)? Ellipsis? RPAREN mixinGuard? block
  ;

mixinCallArgument
  : variableDeclaration
  | valueList
  ;

mixinCall
  : selector (LPAREN (mixinCallArgument (SEMI mixinCallArgument)*)? RPAREN)? IMPORTANT?
  | selector (LPAREN (mixinCallArgument (COMMA mixinCallArgument)*)? RPAREN)? IMPORTANT?
  ;

mixinGuard
  : WHEN mixinGuardConditions
  ;

mixinDefinitionParam
  : variableName
  | variableDeclaration
  | identifier
  | string
  | Number
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
  : (COLON|COLONCOLON) Identifier (LPAREN expression RPAREN)?;

attribRelate
  : EQ 
  | TILD_EQ
  | PIPE_EQ
  | CIRC_EQ
  | DOLLAR_EQ
  | STAR_EQ
  ;

keywordAsIdentifier
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
  | CHAR_UNIT
  | KnownColor
  | EXTEND
  | ALL
  | FROM
  | TO
  ;

variableInterpolation
  : InterpolationStart identifierVariableName BlockEnd 
  ;

identifier
  : (keywordAsIdentifier | Identifier | IdentifierAfter) identifierPart*
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
  : UrlStart (string|variableName|Url) RPAREN
  ;

fraction
  : Number DIV Number Unit?
  ;

measurement
  : Number Unit?
  ;

featureQuery
  : MediaQueryModifier? LPAREN property RPAREN
  | MediaQueryModifier? LPAREN (identifier|variableName) RPAREN
  | MediaQueryModifier? (identifier|variableName)
  ;

mediaQuery
  : featureQuery (AND featureQuery)*
  ;


mediaBlock
  : MEDIA mediaQuery (COMMA mediaQuery)* block
  ;
