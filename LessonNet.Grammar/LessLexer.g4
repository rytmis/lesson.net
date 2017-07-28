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
PARENTREF       : '&' -> pushMode(IDENTIFY);
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
LTEQ_MIXINGUARD : '=<';
NOTEQ           : '!=';
EQ              : '=';
PIPE_EQ         : '|=';
TILD_EQ         : '~=';
CIRC_EQ         : '^=';
DOLLAR_EQ       : '$=';
STAR_EQ         : '*=';

TRUE            : 'true';
FALSE           : 'false';

/* URLs */
/* http://lesscss.org/features/#variables-feature-urls */
URL : 'url';

UrlStart
  : URL LPAREN -> pushMode(URL_STARTED)
  ;

MEDIA           : '@media';
IMPORT          : '@import';
CHARSET         : '@charset';
NAMESPACE       : '@namespace';
SUPPORTS        : '@supports';
DOCUMENT        : '@document';
PAGE            : '@page';
FROM            : 'from';
TO              : 'to';
COUNTERSTYLE    : '@counter-style';
EXTEND          : 'extend';
IMPORTANT       : '!important';
ARGUMENTS       : '@arguments';
REST            : '@rest';
ALL             : 'all';

fragment Vendor
  : 'o'
  | 'moz'
  | 'ms'
  | 'webkit'
  ;

fragment VendorPrefix
  : MINUS Vendor MINUS
  ;

KEYFRAMES
  : VendorPrefix? 'keyframes'
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

KnownColor
  : ( 'aliceblue' | 'antiquewhite' | 'aqua' | 'aquamarine' | 'azure' | 'beige' | 'bisque' | 'black'
    | 'blanchedalmond' | 'blue' | 'blueviolet' | 'brown' | 'burlywood' | 'cadetblue' | 'chartreuse' | 'chocolate'
    | 'coral' | 'cornflowerblue' | 'cornsilk' | 'crimson' | 'cyan' | 'darkblue' | 'darkcyan' | 'darkgoldenrod' 
    | 'darkgray' | 'darkgrey' | 'darkgreen' | 'darkkhaki' | 'darkmagenta' | 'darkolivegreen' | 'darkorange' 
    | 'darkorchid' | 'darkred' | 'darksalmon' | 'darkseagreen' | 'darkslateblue' | 'darkslategray' | 'darkslategrey' 
    | 'darkturquoise' | 'darkviolet' | 'deeppink' | 'deepskyblue' | 'dimgray' | 'dimgrey' | 'dodgerblue' | 'firebrick' 
    | 'floralwhite' | 'forestgreen' | 'fuchsia' | 'gainsboro' | 'ghostwhite' | 'gold' | 'goldenrod' | 'gray' | 'grey' 
    | 'green' | 'greenyellow' | 'honeydew' | 'hotpink' | 'indianred' | 'indigo' | 'ivory' | 'khaki' | 'lavender' 
    | 'lavenderblush' | 'lawngreen' | 'lemonchiffon' | 'lightblue' | 'lightcoral' | 'lightcyan' | 'lightgoldenrodyellow' 
    | 'lightgray' | 'lightgrey' | 'lightgreen' | 'lightpink' | 'lightsalmon' | 'lightseagreen' | 'lightskyblue' 
    | 'lightslategray' | 'lightslategrey' | 'lightsteelblue' | 'lightyellow' | 'lime' | 'limegreen' | 'linen' | 'magenta' 
    | 'maroon' | 'mediumaquamarine' | 'mediumblue' | 'mediumorchid' | 'mediumpurple' | 'mediumseagreen' | 'mediumslateblue' 
    | 'mediumspringgreen' | 'mediumturquoise' | 'mediumvioletred' | 'midnightblue' | 'mintcream' | 'mistyrose' | 'moccasin' 
    | 'navajowhite' | 'navy' | 'oldlace' | 'olive' | 'olivedrab' | 'orange' | 'orangered' | 'orchid' | 'palegoldenrod' 
    | 'palegreen' | 'paleturquoise' | 'palevioletred' | 'papayawhip' | 'peachpuff' | 'peru' | 'pink' | 'plum' | 'powderblue' 
    | 'purple' | 'rebeccapurple' | 'red' | 'rosybrown' | 'royalblue' | 'saddlebrown' | 'salmon' | 'sandybrown' | 'seagreen' 
    | 'seashell' | 'sienna' | 'silver' | 'skyblue' | 'slateblue' | 'slategray' | 'slategrey' | 'snow' | 'springgreen' 
    | 'steelblue' | 'tan' | 'teal' | 'thistle' | 'tomato' | 'turquoise' | 'violet' | 'wheat' | 'white' | 'whitesmoke' 
    | 'yellow' | 'yellowgreen' | 'transparent')
  ;

DQUOT_STRING_START : '"' -> pushMode(DQ_STRING);
SQUOT_STRING_START : '\'' -> pushMode(SQ_STRING);

Number
  :  '-'? (('0'..'9')* '.')? ('0'..'9')+ -> pushMode(NUMBER_STARTED)
  ;

fragment HEXDIGIT
  : ('0'..'9'|'a'..'f'|'A'..'F')
  ;

HexColor
  :  '#' HEXDIGIT HEXDIGIT HEXDIGIT
  |  '#' HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT HEXDIGIT
  ;

/* Whitespace -- ignored */
WS
  : (' '|'\t'|'\n'|'\r'|'\r\n')+ -> channel(HIDDEN)
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

Unit
  : ('px'|'cm'|'mm'|'in'|'pt'|'pc'|'rem'|'em'|'ex'|'deg'|'rad'|'grad'|'ms'|'s'|'hz'|'khz'|'dpi'|'dpcm'|'vmin'|'vmax'|'vm'|'vw'|'vh');

fragment ID_CHAR : ('_' | '-' | 'a'..'z'| 'A'..'Z' | '\u0100'..'\ufffe' | '0'..'9' | '\\.' | '\\:' );

Identifier
  : ( ('_' | 'a'..'z'| 'A'..'Z' | '\u0100'..'\ufffe' ) ID_CHAR*
	|  '-' ('_' | 'a'..'z'| 'A'..'Z' | '\u0100'..'\ufffe' )* ID_CHAR*) -> pushMode(IDENTIFY);


mode URL_STARTED;
DQUOT_STRING_START_URL : '"' -> type(DQUOT_STRING_START), pushMode(DQ_STRING);
SQUOT_STRING_START_URL : '\'' -> type(SQUOT_STRING_START), pushMode(SQ_STRING);
UrlEnd                 : RPAREN -> type(RPAREN), popMode;
URL_AT                 : AT -> type(AT), pushMode(IDENTIFY);
Url                    : ~('\'' | '"' | ')' | '\n' | '\r' | '@') (~(')' | '\n' | '\r' | '@'))+;

// If we switch modes to identify and then encounter a RPAREN, we pop modes to here, but then never popMode back
URL_SEMI               : SEMI -> type(SEMI), popMode;


mode IDENTIFY;
BlockStart_ID             : BlockStart -> popMode, type(BlockStart);
SPACE                  : WS -> popMode, type(WS), channel(HIDDEN);

DOLLAR_ID              : DOLLAR -> type(DOLLAR);

InterpolationStartAfter  : InterpolationStart;
InterpolationEnd_ID    : BlockEnd -> popMode, type(BlockEnd);

IdentifierAfter        : Identifier;
Ellipsis_ID            : Ellipsis -> popMode, type(Ellipsis);
DOT_ID                 : DOT -> popMode, type(DOT);
HASH_ID                : HASH -> popMode, type(HASH);

LPAREN_ID                 : LPAREN -> popMode, type(LPAREN);
RPAREN_ID                 : RPAREN -> popMode, type(RPAREN);

COLONCOLON_ID             : COLONCOLON -> popMode, type(COLONCOLON);
COLON_ID                  : COLON -> popMode, type(COLON);
COMMA_ID                  : COMMA -> popMode, type(COMMA);
SEMI_ID                  : SEMI -> popMode, type(SEMI);
LBRACK_ID              : LBRACK -> popMode, pushMode(ATTRIB), type(LBRACK);
RBRACK_ID              : RBRACK -> popMode, type(RBRACK);
EQ_ID       : EQ -> popMode, type(EQ);
PIPE_EQ_ID  : PIPE_EQ -> popMode, type(PIPE_EQ);
TILD_EQ_ID  : TILD_EQ -> popMode, type(TILD_EQ);
CIRC_EQ_ID  : CIRC_EQ -> popMode, type(CIRC_EQ);
DOLLAR_EQ_ID: DOLLAR_EQ -> popMode, type(DOLLAR_EQ);
STAR_EQ_ID  : STAR_EQ -> popMode, type(STAR_EQ);
PARENTREF_ID: PARENTREF -> popMode, type(PARENTREF);
DIV_ID      : DIV -> popMode, type(DIV);


mode ATTRIB;
AttribIdentifier		: Identifier -> type(Identifier), pushMode(IDENTIFY);
ATTRIB_DQ_STRING_START	: '"' -> type(DQUOT_STRING_START), pushMode(DQ_STRING);
ATTRIB_SQ_STRING_START  : '\'' -> type(SQUOT_STRING_START), pushMode(SQ_STRING);
ATTRIB_EQUAL		: EQ -> type(EQ);
ATTRIB_PIPE_EQUAL	: PIPE_EQ -> type(PIPE_EQ);
ATTRIB_TILD_EQUAL	: TILD_EQ -> type(TILD_EQ);
ATTRIB_CIRC_EQUAL	: CIRC_EQ -> type(CIRC_EQ);
ATTRIB_DOLLAR_EQUAL	: DOLLAR_EQ -> type(DOLLAR_EQ);
ATTRIB_STAR_EQUAL	: STAR_EQ -> type(STAR_EQ);
ATTRIB_RBRACK		: RBRACK -> popMode, type(RBRACK);
ATTRIB_WS			: WS -> popMode, type(WS), channel(HIDDEN);
ATTRIB_BlockStart	: BlockStart -> popMode, type(BlockStart);

mode NUMBER_STARTED;
NUMBER_UNIT: Unit -> popMode, type(Unit);
NUMBER_IDENTIFIER : Identifier -> popMode, type(Identifier), pushMode(IDENTIFY);
NUMBER_SPACE : ' ' -> popMode;
NUMBER_SEMI : SEMI -> popMode, type(SEMI);
NUMBER_RPAREN : RPAREN -> popMode, type(RPAREN);
NUMBER_COMMA : COMMA -> popMode, type(COMMA);
NUMBER_TIMES : TIMES -> popMode, type(TIMES);
NUMBER_PLUS : PLUS -> popMode, type(PLUS);
NUMBER_DIV : DIV -> popMode, type(DIV);
NUMBER_MINUS : MINUS -> popMode, type(MINUS);
NUMBER_PERC : PERC -> popMode, type(PERC);

mode SQ_STRING;
SQ_INTERPOLATION_START: InterpolationStart -> type(InterpolationStart), pushMode(IDENTIFY);
SQUOT_STRING_FRAGMENT : (~('\''|'\n'|'\r'|'@'))+;
SQUOT_STRING_END : ('\''|'\n'|'\r') -> popMode;

mode DQ_STRING;
DQ_INTERPOLATION_START: InterpolationStart -> type(InterpolationStart), pushMode(IDENTIFY);
DQUOT_STRING_FRAGMENT : (~('"'|'\n'|'\r'|'@'))+;
DQUOT_STRING_END : ('"'|'\n'|'\r') -> popMode;

