from dataclasses import dataclass, field
from pathlib import Path
from typing import Callable, Iterator
from argparse import ArgumentParser
from mako.template import Template

ROOT_D = Path(__file__).parent.parent.resolve()

KEYWORDS = {
    'abstract', 'as', 'base', 'bool', 'break', 'byte', 'case', 'catch', 'char', 'checked', 'class', 'const', 'continue',
    'decimal', 'default', 'delegate', 'do', 'double', 'else', 'enum', 'event', 'explicit', 'extern', 'false', 'finally',
    'fixed', 'float', 'for', 'foreach', 'goto', 'if', 'implicit', 'in', 'in', 'int', 'interface', 'internal', 'is',
    'lock', 'long', 'namespace', 'new', 'null', 'object', 'operator', 'out', 'out', 'override', 'record',
    'params', 'private', 'protected', 'public', 'readonly', 'ref', 'return', 'sbyte', 'sealed', 'short', 'sizeof',
    'stackalloc', 'static', 'string', 'struct', 'switch', 'this', 'throw', 'true', 'try', 'typeof', 'uint', 'ulong',
    'unchecked', 'unsafe', 'ushort', 'using', 'using', 'static', 'virtual', 'void', 'volatile', 'while'
}


@dataclass
class Tree:
    func: Callable[[], "AstBuilder"]
    dest: Path


@dataclass
class Prop:
    name: str
    type: str
    init: bool = True

    @property
    def arg_name(self) -> str:
        name = f"{self.name[0].lower()}{self.name[1:]}"
        if name in KEYWORDS:
            return f"@{name}"
        return name


@dataclass
class Node:
    name: str
    props: list[Prop]

    @property
    def init_props(self) -> Iterator[Prop]:
        for p in self.props:
            if p.init:
                yield p

    @property
    def ctor_args(self) -> Iterator[str]:
        for p in self.init_props:
            yield f"{p.type} {p.arg_name}"

    def __hash__(self) -> int:
        return hash(self.name)

    @property
    def arg_name(self) -> str:
        name = f"{self.name[0].lower()}{self.name[1:]}"
        if name in KEYWORDS:
            return f"@{name}"
        return name

@dataclass
class AstBuilder:
    namespace: str
    root_class: str
    usings: set[str] = field(default_factory=set)
    nodes: set[Node] = field(default_factory=set)

    def using(self, *usings: str):
        self.usings |= set(usings)

    def add_node(self, name: str, *props: Prop):
        node = Node(name, list(props))
        self.nodes.add(node)


def build_expr() -> AstBuilder:
    builder = AstBuilder("LoxSharp.Parsing", "Expr")
    builder.using("System", "System.Collections.Generic", "LoxSharp.Lexing")
    builder.add_node(
        "Assignment",
        Prop("Left", "Token"),
        Prop("Value", "Expr")
    )
    builder.add_node("Comma", Prop("Values", "List<Expr>"))
    builder.add_node(
        "Conditional",
        Prop("Condition", "Expr"),
        Prop("Then", "Expr"),
        Prop("Else", "Expr")
    )
    builder.add_node(
        "Logic",
        Prop("Left", "Expr"),
        Prop("Operator", "Token"),
        Prop("Right", "Expr")
    )
    builder.add_node(
        "Binary",
        Prop("Left", "Expr"),
        Prop("Operator", "Token"),
        Prop("Right", "Expr")
    )
    builder.add_node(
        "Unary",
        Prop("Left", "Expr"),
        Prop("Operator", "Token"),
        Prop("Right", "Expr")
    )
    builder.add_node(
        "Call",
        Prop("Callee", "Expr"),
        Prop("Paren", "Token"),
        Prop("Arguments", "List<Expr>")
    )
    builder.add_node(
        "Get",
        Prop("Object", "Expr"),
        Prop("Name", "Token")
    )
    builder.add_node(
        "Set",
        Prop("Object", "Expr"),
        Prop("Name", "Token"),
        Prop("Value", "Expr")
    )
    builder.add_node(
        "Lambda",
        Prop("Keyword", "Token"),
        Prop("Parameters", "List<Token>"),
        Prop("Body", "Expr")
    )
    builder.add_node("Grouping", Prop("Expr", "Expr"))
    builder.add_node("Variable", Prop("Name", "Token"))
    builder.add_node("Literal", Prop("Value", "object"))
    return builder


def parse_cli_args():
    parser = ArgumentParser("gen_ast", description="Generates an AST")
    parser.add_argument("tree", help="The name of the tree to generate")
    return parser.parse_args()


TREES = {
    "expr": Tree(build_expr, ROOT_D / "Parsing" / "Expr.cs")
}


def main():
    args = parse_cli_args()
    tree = TREES[args.tree]
    ast = tree.func()
    templ = Template(filename=str(ROOT_D / "Scripts" / "ast.mako"))
    code = templ.render(ast=ast)
    with tree.dest.open("w") as f:
        f.write(code)


if __name__ == "__main__":
    main()
