grammar ScimFilter;

/** Parser: will organize the tokens provided by the lexer into the tree with sub-trees and also defining the precedence of the operators */
parse
    : expression
    ;

expression
    : expression OR expression    #orExp
    | expression AND expression   #andExp
    | NOT '(' expression ')'      #notExp
    | '(' expression ')'          #braceExp
	| FIELD '[' expression ']'	  #bracketExp
    | FIELD PRESENT               #presentExp
    | FIELD OPERATOR VALUE        #operatorExp
    ;

/** Lexer: Interpreting character and turn them into tokens which will be organized by the parser in an tree */
OR
    : 'or'
    | 'Or'
    | 'oR'
    | 'OR'
    ;

AND
    : 'and'
    | 'And'
    | 'aNd'
    | 'anD'
    | 'ANd'
    | 'aND'
    | 'AND'
    ;

NOT
    : 'not'
    | 'Not'
    | 'nOt'
    | 'noT'
    | 'NOt'
    | 'nOT'
    | 'NOT'
    ;

PRESENT
    : 'pr'
    ;

OPERATOR
    : 'eq'
	| 'ne'
    | 'co'
	| 'sw'
	| 'ew'
    | 'gt'
    | 'ge'
    | 'lt'
    | 'le'
    ;

FIELD
    : ([a-z] | [A-Z] | [0-9] | '.' | ':' | '_' | '-')+ //match lower-case, upper-case, number identifier including dots, colons and quotation marks
    ;

ESCAPED_QUOTE
    : '\\"'
    ;

VALUE
    : '"'(ESCAPED_QUOTE | ~'"')*'"'
    ;

EXCLUDE
    : [\b | \t | \r | \n]+ -> skip //skipping backspaces, tabs, carriage returns and newlines
    ;