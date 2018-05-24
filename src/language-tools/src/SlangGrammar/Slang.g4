grammar Slang;

/*
 * Parser Rules
 */

/* Типы */
type
	: scalarType
	| arrayType
	;

scalarType
	: simpleType
	| routineType
	;

simpleType
	: SimpleType
	;

routineType
	: funcType
	| procType
	;

funcType
	: Func routineArgList ':' type
	;

procType
	: Proc routineArgList
	;

routineArgList
	: RoutineLeftBracket (routineArg (',' routineArg)* | /* Нет аргументов */ ) RoutineRightBracket
	;

routineArg
	: ArgPassModifier type
	;

arrayType
	: Array (arrayDimention)+ scalarType
	;

arrayDimention
	: ArrayLeftBracket ArrayRightBracket
	;

/* Модуль */
start /* СТАРТОВЫЙ НЕТЕРМИНАЛ */
	: moduleImports module
	;

moduleImports
	: (moduleImport | raw)*
	;

moduleImport
	: Import Id
	;

module
	: Module Id moduleDeclare moduleEntry
	;

moduleDeclare
	: (funcDeclare | procDeclare | raw)*
	;

funcDeclare
	: AccessModifier Func routineDeclareArgList ':' type Id statementSequence End
	;

procDeclare
	: AccessModifier Proc routineDeclareArgList Id statementSequence End
	;

routineDeclareArgList
	: RoutineLeftBracket (routineDeclareArg (',' routineDeclareArg)* | /* нет аргументов */ )  RoutineRightBracket
	;

routineDeclareArg
	: ArgPassModifier type Id
	;

moduleEntry
	: Start statementSequence End
	;

statementSequence
	: (statement | raw)*
	;

statement
	: singleStatement
	| multiStatement
	;

singleStatement
	: declare 
	| assign 
	| input
	| output 
	| return
	| call
	;

multiStatement
	: if
	| whileLoop
	| doWhileLoop
	;

declare
	: constDeclare
	| scalarDeclare 
	| arrayDeclare
	;

constDeclare
	: 'const' simpleType Id Assign (mathExp | boolOr)
	;

scalarDeclare
	: scalarType Id (Assign mathExp | Assign boolOr)?
	;

arrayDeclare
	: arrayDeclareType Id
	;

arrayDeclareType
	: Array (arrayDeclareDimention)+ scalarType
	;

arrayDeclareDimention
	: ArrayLeftBracket mathExp ArrayRightBracket
	;

arrayElement
	: Id (arrayDeclareDimention)+
	;

arrayLength
	: Id '.' 'length' RoutineLeftBracket IntValue RoutineRightBracket
	;

assign
	: singleAssign
	| arrayAssign
	;

singleAssign
	: Id Assign mathExp
	| Id Assign boolOr
	| Id Assign assign
	;

arrayAssign
	: arrayElement Assign mathExp
	| arrayElement Assign boolOr
	| arrayElement Assign assign
	;

return
	: 'return' (exp)?
	;

input
	: 'input' Id
	;

output
	: 'output' outputOperand (',' outputOperand)*
	;

outputOperand
	: StringLiteral | exp
	;

call // Вызов процедуры/функции
	: 'call' id RoutineLeftBracket callArgList RoutineRightBracket
	;

callArgList
	: (callArg (',' callArg)*) 
	| /* нет аргументов */
	;

callArg
	: exp
	;

if
	: 'if' '(' boolOr ')' 'then' statementSequence End #IfSingle
	| 'if' '(' boolOr ')' 'then' statementSequence 'else' statementSequence End #IfElse
	;

whileLoop
	: 'while' '(' boolOr ')' 'repeat' statementSequence End
	;

doWhileLoop
	: 'repeat' statementSequence 'while' '(' boolOr ')'
	;

mathExp
	: mathTerm #MathExpEmpty
	| mathTerm '+' mathExp #MathExpSum 
	| mathTerm '-' mathExp #MathExpSub
	;

mathTerm
	: mathFactor #MathTermEmpty
	| mathFactor '*' mathTerm #MathTermMul 
	| mathFactor '/' mathTerm #MathTermDiv 
	| mathFactor '%' mathTerm #MathTermMod
	;

mathFactor
	: expAtom #MathFactorEmpty
	| '(' mathExp ')' #MathFactorBrackets
	| '+' mathFactor #MathFactorUnaryPlus
	| '-' mathFactor #MathFactorUnaryMinus
	;

boolOr
	: boolAnd #BoolOrEmpty
	| boolAnd '||' boolOr #LogicOr
	;

boolAnd
	: boolEquality #BoolAndEmpty
	| boolEquality '&&' boolAnd #LogicAnd
	;

boolEquality
	: boolInequality #BoolEqualityEmpty
	| mathExp '==' mathExp #MathEqual
	| boolInequality '==' boolEquality #BoolEqual
	| mathExp '!=' mathExp #MathNotEqual
	| boolInequality '!=' boolEquality #BoolNotEqual 
	;

boolInequality
	: boolFactor #BoolInequalityEmpty
	| mathExp '>' mathExp #Bigger
	| mathExp '<' mathExp #Lesser
	| mathExp '>=' mathExp #BiggerOrEqual
	| mathExp '<=' mathExp #LesserOrEqual
	;

boolFactor
	: expAtom #BoolAtomEmpty 
	| '!' expAtom #Not 
	| '(' boolOr ')' #BoolAtomBrackets 
	| '!' '(' boolOr ')' #BoolAtomBracketsNot
	;

expAtom
	: call
	| arrayLength
	| arrayElement
	| id
	| IntValue
	| RealValue
	| BoolValue
	;

id
	: (Id '::')? Id
	;

exp
	: mathExp 
	| boolOr
	;

raw
	: 'raw' any End
	;

any
	: (.)*?
	;

/*
 * Lexer Rules
 */

SimpleType
	: Int 
	| Real 
	| Bool
	;

fragment 
Int
	: 'int'
	;

fragment 
Real
	: 'real'
	;

fragment 
Bool
	: 'bool'
	;

Array
	: 'array'
	;

ArrayLeftBracket
	: '['
	;

ArrayRightBracket
	: ']'
	;

Func
	: 'fun'
	;

Proc
	: 'proc'
	;

RoutineLeftBracket
	: '('
	;

RoutineRightBracket
	: ')'
	;

Import
	: 'import'
	;

Module
	: 'module'
	;

Start
	: 'start'
	;

End
	: 'end'
	;

ArgPassModifier
	: ValPassModifier
	| RefPassModifier
	;

fragment 
ValPassModifier
	: 'val'
	;

fragment 
RefPassModifier
	: 'ref'
	;

AccessModifier
	: PublicModifier
	| PrivateModifier;

fragment 
PublicModifier
	: 'public'
	;

fragment 
PrivateModifier
	: 'private'
	;

Assign
	: '='
	;

IntValue
	: Digit+
	;

RealValue
	: Digit*'.'?Digit+([eE][-+]?Digit+)?
	;

fragment 
Digit
	: [0-9]
	;

BoolValue
	: 'true' 
	| 'false'
	;

Id
	: [_a-zA-Z][_a-zA-Z0-9]*
	;

StringLiteral
	:	'"' StringCharacter* '"'
	;

fragment
StringCharacter
	:	~["]
	|	EscapeSequence
	;

fragment
EscapeSequence
	:	'\\' [btnfr"'\\]
	;

T: '{' | '}' | '#' | ';' ;

Comment
	: ('//' ~[\r\n]* | '/*' .*? '*/') 
	-> skip
	;
Ws
	: [ \t\r\n] 
	-> skip
	;