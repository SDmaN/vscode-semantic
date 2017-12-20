grammar Slang;

/*
 * Parser Rules
 */

 start: moduleImports module;

 moduleImports: ('import' Id)*;

 module: 'module' Id moduleBlock;
 moduleBlock: BeginBlock (func | proc)* EndBlock;

 func: AccessModifier 'fun' Type Id '(' argList ')' statementBlock;
 proc: AccessModifier 'proc' Id '(' argList ')' statementBlock;
 argList: Type Id (',' Type Id)* | /* нет аргументов */ ;

 statementBlock: BeginBlock statementSequence EndBlock;
 statementSequence: (statement)*;
 statement: declare | let | input | output | return;

 declare: Type Id ('=' mathExp | '=' boolOr)?;
 let: Id '=' mathExp | Id '=' boolOr | Id '=' let;
 return: 'return' (mathExp | boolOr);

 input: 'input' Id;
 output: 'output' (mathExp | boolOr);

 mathExp: mathTerm #MathExpEmpty | mathTerm '+' mathExp #MathExpSum | mathTerm '-' mathExp #MathExpSub;
 mathTerm: mathFactor #MathTermEmpty | mathFactor '*' mathTerm #MathTermMul | mathFactor '/' mathTerm #MathTermDiv | mathFactor '%' mathTerm #MathTermMod;
 mathFactor : mathAtom #MathFactorEmpty | '(' mathExp ')' #MathFactorBrackets | '+' mathFactor #MathFactorUnaryPlus | '-' mathFactor #MathFactorUnaryMinus;
 mathAtom: IntValue | RealValue | Id;

boolOr: boolAnd #BoolOrEmpty | boolAnd '||' boolOr #LogicOr;
boolAnd: boolEquality #BoolAndEmpty | boolEquality '&&' boolAnd #LogicAnd;
boolEquality: boolInequality #BoolEqualityEmpty | boolInequality '==' boolEquality #BoolEqual | mathExp '==' mathExp #MathEqual | boolInequality '!=' boolEquality #BoolNotEqual | mathExp '!=' mathExp #MathNotEqual;
boolInequality: boolFactor #BoolInequalityEmpty | mathExp '>' mathExp #Bigger | mathExp '<' mathExp #Lesser | mathExp '>=' mathExp #BiggerOrEqual | mathExp '<=' mathExp #LesserOrEqual;
boolFactor: boolAtom #BoolAtomEmpty | '!' boolAtom #Not | '(' boolOr ')' #BoolAtomBrackets | '!' '(' boolOr ')' #BoolAtomBracketsNot;
boolAtom: Bool | Id;

/*
 * Lexer Rules
 */

 BeginBlock: 'begin';
 EndBlock: 'end';

 Type: Int | Real | Bool;
 Int: 'int';
 Real: 'float';
 Bool: 'bool';

 AccessModifier: PublicModifier | InternalModifier;

 PublicModifier: 'public';
 InternalModifier: 'internal';

 Id: [_a-zA-Z][_a-zA-Z0-9]*;

 IntValue: Digit+;
 RealValue: [0-9]*'.'?[0-9]+([eE][-+]?[0-9]+)?;
 fragment Digit: [0-9];

 Comment: ('//' ~[\r\n]* | '/*' .*? '*/') -> skip;
 Ws: [ \t\r\n] -> skip;