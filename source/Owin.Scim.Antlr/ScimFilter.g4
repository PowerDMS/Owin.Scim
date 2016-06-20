grammar ScimFilter;

parse
    : filter
    ;

filter
	: FIELD SP PR				#presentExp
	| FIELD SP COMPAREOPERATOR SP VALUE	#operatorExp
	| NOT? SP* '(' filter ')'		#braceExp
	| FIELD '[' valPathFilter ']'		#valPathExp
	| filter SP AND SP filter		#andExp
	| filter SP OR SP filter		#orExp
	;

valPathFilter
	: FIELD SP PR				#valPathPresentExp
	| FIELD SP COMPAREOPERATOR SP VALUE	#valPathOperatorExp
	| NOT? SP* '(' valPathFilter ')'	#valPathBraceExp
	| valPathFilter SP AND SP valPathFilter	#valPathAndExp
	| valPathFilter SP OR SP valPathFilter	#valPathOrExp
	;
	
COMPAREOPERATOR : EQ | NE | CO | SW | EW | GT | GE | LT | LE;

EQ : [eE][qQ];
NE : [nN][eE];
CO : [cC][oO];
SW : [sS][wW];
EW : [eE][wW];
PR : [pP][rR];
GT : [gG][tT];
GE : [gG][eE];
LT : [lL][tT];
LE : [lL][eE];

NOT : [nN][oO][tT];
AND : [aA][nN][dD];
OR  : [oO][rR];

SP : ' ';

FIELD : ([a-z] | [A-Z] | [0-9] | '.' | ':' | '_' | '-')+;

ESCAPED_QUOTE : '\\"';

VALUE : '"'(ESCAPED_QUOTE | ~'"')*'"';

EXCLUDE : [\b | \t | \r | \n]+ -> skip;
