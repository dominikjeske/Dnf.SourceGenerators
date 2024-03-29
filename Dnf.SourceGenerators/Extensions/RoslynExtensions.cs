﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Dnf.SourceGenerators
{
    public static class RoslynExtensions
    {
        public static IEnumerable<INamedTypeSymbol> GetAllWithBaseClass(this Compilation compilation, string baseClass, CancellationToken token)
        {
            var visitor = new TypeVisitor(type => type.TypeKind == TypeKind.Class &&
                                                 type.GetBaseTypes().Any(x => x.ToDisplayString() == baseClass
                                        ), token);
            visitor.Visit(compilation.GlobalNamespace);

            return visitor.GetResults();
        }

        public static IEnumerable<ITypeSymbol> GetBaseTypesAndThis(this ITypeSymbol type)
        {
            var current = type;
            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }

        public static IEnumerable<ITypeSymbol> GetBaseTypes(this ITypeSymbol type)
        {
            var current = type.BaseType;
            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }

        public static IEnumerable<ISymbol> GetAllMembers(this ITypeSymbol type)
        {
            return type.GetBaseTypesAndThis().SelectMany(n => n.GetMembers());
        }

        public static CompilationUnitSyntax GetCompilationUnit(this SyntaxNode syntaxNode)
        {
            return syntaxNode.Ancestors().OfType<CompilationUnitSyntax>().FirstOrDefault();
        }

        public static string GetClassName(this ClassDeclarationSyntax proxy) => proxy.Identifier.Text;

        public static string GetClassModifier(this ClassDeclarationSyntax proxy) => proxy.Modifiers.ToFullString().Trim();

        public static bool HaveAttribute(this ClassDeclarationSyntax classSyntax, string attributeName)
        {
            return classSyntax.AttributeLists.Count > 0 &&
                   classSyntax.AttributeLists.SelectMany(al => al.Attributes
                                             .Where(a => (a.Name as IdentifierNameSyntax)?.Identifier.Text == attributeName))
                                             .Any();
        }

        public static string GetNamespace(this CompilationUnitSyntax root)
        {
            return root.ChildNodes()
                       .OfType<NamespaceDeclarationSyntax>()
                       .FirstOrDefault()
                       .Name
                       .ToString();
        }

        public static List<string> GetUsings(this CompilationUnitSyntax root)
        {
            return root.ChildNodes()
                       .OfType<UsingDirectiveSyntax>()
                       .Select(n => n.Name.ToString())
                       .ToList();
        }
    }
}