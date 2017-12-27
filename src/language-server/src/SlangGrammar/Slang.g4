grammar Slang;

/*
 * Parser Rules
 */

start: moduleImports module;

moduleImports: ('import' Id)*;

module: 'module' Id moduleBlock;
moduleBlock: BeginBlock (func | proc)* EndBlock;

arrayOrSimpleType: (arrayType | Type);

func: ModuleAccessModifier 'fun' arrayOrSimpleType Id '(' argList ')' statementBlock;
proc: ModuleAccessModifier 'proc' Id '(' argList ')' statementBlock;
argList: argPassModifier arrayOrSimpleType Id (',' argPassModifier arrayOrSimpleType Id)* | /* нет аргументов */ ;
argPassModifier : ArgPassModifier;

statementBlock: BeginBlock statementSequence EndBlock;
statementSequence: (statement)*;
statement: declare | arrayDeclare | assign | input | output | return | call | if | whileLoop | doWhileLoop;

declare: arrayOrSimpleType Id ('=' mathExp | '=' boolOr | '=' arrayDeclare)?;

arrayType: Type ArrayTypeBrackets (ArrayTypeBrackets)*;
arrayDeclare: NewKeyword Type '[' mathExp ']' ('[' mathExp ']')*;
arrayElement: Id '[' mathExp ']' ('[' mathExp ']')*;

assign: singleAssign | arrayAssign;
singleAssign: Id '=' mathExp | Id '=' boolOr | Id '=' assign;
arrayAssign: arrayElement '=' mathExp | arrayElement '=' boolOr | arrayElement '=' assign;

return: 'return' (mathExp | boolOr)?;

input: 'input' Id;
output: 'output' (mathExp | boolOr);

call: 'call' (Id '::')? Id '(' callArgList ')'; // Вызов процедуры/функции
callArgList: ((callArg) (',' (callArg))*) | /* нет аргументов */ ;
callArg: mathExp | boolOr;

if: 'if' '(' boolOr ')' statementBlock #IfSingle | 'if' '(' boolOr ')' statementBlock 'else' statementBlock #IfElse;
whileLoop: 'while' '(' boolOr ')' statementBlock;
doWhileLoop: 'do' statementBlock 'while' '(' boolOr ')';

mathExp: mathTerm #MathExpEmpty | mathTerm '+' mathExp #MathExpSum | mathTerm '-' mathExp #MathExpSub;
mathTerm: mathFactor #MathTermEmpty | mathFactor '*' mathTerm #MathTermMul | mathFactor '/' mathTerm #MathTermDiv | mathFactor '%' mathTerm #MathTermMod;
mathFactor : mathAtom #MathFactorEmpty | '(' mathExp ')' #MathFactorBrackets | '+' mathFactor #MathFactorUnaryPlus | '-' mathFactor #MathFactorUnaryMinus;
mathAtom: call | arrayElement | IntValue | RealValue | Id;

boolOr: boolAnd #BoolOrEmpty | boolAnd '||' boolOr #LogicOr;
boolAnd: boolEquality #BoolAndEmpty | boolEquality '&&' boolAnd #LogicAnd;
boolEquality: boolInequality #BoolEqualityEmpty | boolInequality '==' boolEquality #BoolEqual | mathExp '==' mathExp #MathEqual | boolInequality '!=' boolEquality #BoolNotEqual | mathExp '!=' mathExp #MathNotEqual;
boolInequality: boolFactor #BoolInequalityEmpty | mathExp '>' mathExp #Bigger | mathExp '<' mathExp #Lesser | mathExp '>=' mathExp #BiggerOrEqual | mathExp '<=' mathExp #LesserOrEqual;
boolFactor: boolAtom #BoolAtomEmpty | '!' boolAtom #Not | '(' boolOr ')' #BoolAtomBrackets | '!' '(' boolOr ')' #BoolAtomBracketsNot;
boolAtom: call | arrayElement | BoolValue | Id;

/*
 * Lexer Rules
 */

BeginBlock: 'begin';
EndBlock: 'end';

Type: Int | Real | Bool;
Int: 'int';
Real: 'real';
Bool: 'bool';

ArrayTypeBrackets: '[' ']';

NewKeyword: 'new';

ArgPassModifier: 'val' | 'ref';
ModuleAccessModifier: PublicModifier | InternalModifier;
ClassMemberAccessModifier: PublicModifier  | PrivateModifier;

fragment PublicModifier: 'public';
fragment InternalModifier: 'internal';
fragment PrivateModifier: 'private';

Heritable: 'heritable';

Id: [_a-zA-Z][_a-zA-Z0-9]*;

IntValue: Digit+;
RealValue: Digit*'.'?Digit+([eE][-+]?Digit+)?;
fragment Digit: [0-9];

BoolValue: 'true' | 'false';

fragment Symbol: [a-zA-Z];
fragment Escape: [\t\r\n];

Comment: ('//' ~[\r\n]* | '/*' .*? '*/') -> skip;
Ws: [ \t\r\n] -> skip;