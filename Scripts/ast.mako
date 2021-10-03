%for using in ast.usings:
using ${using};
%endfor

namespace ${ast.namespace}
{
    public interface I${ast.root_class}Visitor
    {
        %for node in ast.nodes:
        public void Visit${node.name}(${ast.root_class}.${node.name} ${node.arg_name});
        %endfor
    }
    public interface I${ast.root_class}Visitor<T>
    {
        %for node in ast.nodes:
        public T Visit${node.name}(${ast.root_class}.${node.name} ${node.arg_name});
        %endfor
    }
    public abstract partial class ${ast.root_class}
    {

        public abstract void Accept(I${ast.root_class}Visitor visitor);
        public abstract T Accept<T>(I${ast.root_class}Visitor<T> visitor);

        %for node in ast.nodes:
            public sealed partial class ${node.name}: ${ast.root_class}
            {
                %for prop in node.props:
                public ${prop.type} ${prop.name} { get; set; }
                %endfor
                public ${node.name}(${", ".join(node.ctor_args)})
                {
                    %for prop in node.props:
                    ${prop.name} = ${prop.arg_name};
                    %endfor
                }
                public override void Accept(I${ast.root_class}Visitor visitor) => visitor.Visit${node.name}(this);
                public override T Accept<T>(I${ast.root_class}Visitor<T> visitor) => visitor.Visit${node.name}(this);
            }
        %endfor
    }
}
