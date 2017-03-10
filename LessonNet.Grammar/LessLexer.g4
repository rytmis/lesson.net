/*
 [The "MIT License"]
 Copyright (c) 2014 Kyle Lee
 All rights reserved.
*/

lexer grammar LessLexer;

NULL: 'null';


IN: 'in';

Ellipsis: '...';

InterpolationStart
  : AT BlockStart -> pushMode(IDENTIFY)
  ;

/* Separators */
LPAREN          : '(';
RPAREN          : ')';
BlockStart      : '{';
BlockEnd        : '}';
LBRACK          : '[';
RBRACK          : ']';
GT              : '>';
TIL             : '~';

LT              : '<';
COLON           : ':';
SEMI            : ';';
COMMA           : ',';
DOT             : '.';
DOLLAR          : '$';
AT              : '@';
PARENTREF       : '&';
HASH            : '#';
COLONCOLON      : '::';
PLUS            : '+';
TIMES           : '*';
DIV             : '/';
MINUS           : '-';
PERC            : '%';

EQEQ            : '==';
GTEQ            : '>=';
LTEQ            : '<=';
NOTEQ           : '!=';
EQ              : '=';
PIPE_EQ         : '|=';
TILD_EQ         : '~=';

/* URLs */
/* http://lesscss.org/features/#variables-feature-urls */
URL : 'url';

UrlStart
  : URL LPAREN -> pushMode(URL_STARTED)
  ;

MEDIA           : '@media';
IMPORT          : '@import';
EXTEND          : ':extend';
IMPORTANT       : '!important';
ARGUMENTS       : '@arguments';
REST            : '@rest';

MediaQueryModifier
  : (NOT|ONLY)
  ;

/* media options */
ONLY: 'only';

/* Import options */
/* http://lesscss.org/features/#import-options */
REFERENCE : 'reference';
INLINE : 'inline';
LESS : 'less';
CSS : 'css';
ONCE : 'once';
MULTIPLE: 'multiple';

/* Mixin Guards */
WHEN : 'when';
NOT : 'not';
AND : 'and';

Identifier
  : (('_' | 'a'..'z'| 'A'..'Z' | '\u0100'..'\ufffe' )
    ('_' | '-' | 'a'..'z'| 'A'..'Z' | '\u0100'..'\ufffe' | '0'..'9')*
  |  '-' ('_' | 'a'..'z'| 'A'..'Z' | '\u0100'..'\ufffe' )
    ('_' | '-' | 'a'..'z'| 'A'..'Z' | '\u0100'..'\ufffe' | '0'..'9')*) -> pushMode(IDENTIFY)
  ;

fragment STRING
  :  '"' (~('"'|'\n'|'\r'))* '"'
  |  '\'' (~('\''|'\n'|'\r'))* '\''
  ;

/* string literals */
StringLiteral
  :  STRING
  ;

Number
  :  '-'? (('0'..'9')* '.')? ('0'..'9')+ -> pushMode(NUMBER_STARTED)
  ;

Color
  :  '#' ('0'..'9'|'a'..'f'|'A'..'F')+
  ;


/* Whitespace -- ignored */
WS
  : (' '|'\t'|'\n'|'\r'|'\r\n')+ -> skip
  ;

/* Single-line comments */
SL_COMMENT
  :  '//'
    (~('\n'|'\r'))* ('\n'|'\r'('\n')?) -> skip
  ;

/* multiple-line comments */
COMMENT
  :  '/*' .*? '*/' -> skip
  ;

/* Function reference */
/* Misc http://lesscss.org/functions/#misc-functions */
COLOR:'color';
CONVERT:'convert';
DATA_URI:'data-uri';
DEFAULT_FN:'default';
UNIT:'unit';
GET_UNIT:'get-unit';
SVG_GRADIENT:'svg-gradient';

/* String http://lesscss.org/functions/#string-functions */
ESCAPE : 'escape';
E_FN: 'e';
REPLACE : 'replace';

/* List http://lesscss.org/functions/#list-functions */
LENGTH: 'length';
EXTRACT: 'extract';

/* Math http://lesscss.org/functions/#math-functions */
CEIL: 'ceil';
FLOOR: 'floor';
PERCENTAGE: 'percentage';
ROUND: 'round';
SQRT: 'sqrt';
ABS: 'abs';
SIN: 'sin';
ASIN: 'asin';
COS: 'cos';
ACOS: 'acos';
TAN: 'tan';
ATAN: 'atan';
PI: 'pi';
POW: 'pow';
MOD: 'mod';
MIN: 'min';
MAX: 'max';

/* Type http://lesscss.org/functions/#type-functions */
ISNUMBER: 'isnumber';
ISSTRING: 'isstring';
ISCOLOR: 'iscolor';
ISKEYWORD: 'iskeyword';
ISURL: 'isurl';
ISPIXEL: 'ispixel';
ISEM: 'isem';
ISPERCENTAGE: 'ispercentage';
ISUNIT: 'isunit';

/* Color http://lesscss.org/functions/#color-definition */
RGB: 'rgb';
RGBA: 'rgba';
ARGB: 'argb';
HSL: 'hsl';
HSLA: 'hsla';
HSV: 'hsv';
HSVA: 'hsva';

/* Color channel http://lesscss.org/functions/#color-channel */
HUE: 'hue';
SATURATION: 'saturation';
LIGHTNESS: 'lightness';
HSVHUE: 'hsvhue';
HSVSATURATION: 'hsvsaturation';
HSVVALUE: 'hsvvalue';
RED: 'red';
GREEN: 'green';
BLUE: 'blue';
ALPHA: 'alpha';
LUMA: 'luma';
LUMINANCE: 'luminance';

/* Color operation http://lesscss.org/functions/#color-operations */
SATURATE: 'saturate';
DESATURATE: 'desaturate';
LIGHTEN: 'lighten';
DARKEN: 'darken';
FADEIN: 'fadein';
FADEOUT: 'fadeout';
FADE: 'fade';
SPIN: 'spin';
MIX: 'mix';
GREYSCALE: 'greyscale';
CONTRAST: 'contrast';

/* Color blending http://lesscss.org/functions/#color-blending */
MULTIPLY: 'multiply';
SCREEN: 'screen';
OVERLAY: 'overlay';
SOFTLIGHT: 'softlight';
HARDLIGHT: 'hardlight';
DIFFERENCE: 'difference';
EXCLUSION: 'exclusion';
AVERAGE: 'average';
NEGATION: 'negation';

Unit : ('%'|'px'|'cm'|'mm'|'in'|'pt'|'pc'|'em'|'ex'|'deg'|'rad'|'grad'|'ms'|'s'|'hz'|'khz'|'dpi'|'dpcm');

mode URL_STARTED;
UrlEnd                 : RPAREN -> popMode;
Url                    :  STRING | (~(')' | '\n' | '\r' | ';'))+;

mode IDENTIFY;
BlockStart_ID             : BlockStart -> popMode, type(BlockStart);
SPACE                  : WS -> popMode, skip;
DOLLAR_ID              : DOLLAR -> type(DOLLAR);

InterpolationStartAfter  : InterpolationStart;
InterpolationEnd_ID    : BlockEnd -> type(BlockEnd);

IdentifierAfter        : Identifier;
Ellipsis_ID            : Ellipsis -> popMode, type(Ellipsis);
DOT_ID                 : DOT -> popMode, type(DOT);

LPAREN_ID                 : LPAREN -> popMode, type(LPAREN);
RPAREN_ID                 : RPAREN -> popMode, type(RPAREN);

COLON_ID                  : COLON -> popMode, type(COLON);
COMMA_ID                  : COMMA -> popMode, type(COMMA);
SEMI_ID                  : SEMI -> popMode, type(SEMI);
LBRACK_ID              : LBRACK -> popMode, pushMode(ATTRIB), type(LBRACK);
RBRACK_ID              : RBRACK -> popMode, type(RBRACK);

mode ATTRIB;
AttribIdentifier	: Identifier -> popMode, type(Identifier);
ATTRIB_EQUAL		: EQ -> type(EQ);

mode NUMBER_STARTED;
NUMBER_UNIT: Unit -> popMode, type(Unit);
NUMBER_WS : WS -> popMode, skip;
NUMBER_SEMI : SEMI -> popMode, type(SEMI);
NUMBER_RPAREN : RPAREN -> popMode, type(RPAREN);
NUMBER_COMMA : COMMA -> popMode, type(COMMA);

