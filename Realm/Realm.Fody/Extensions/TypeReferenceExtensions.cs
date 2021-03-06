﻿////////////////////////////////////////////////////////////////////////////
//
// Copyright 2016 Realm Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
////////////////////////////////////////////////////////////////////////////

using System.ComponentModel;
using Mono.Cecil;

[EditorBrowsable(EditorBrowsableState.Never)]
internal static class TypeReferenceExtensions
{
    public static bool IsDescendedFrom(this TypeReference @this, TypeReference @base)
    {
        TypeDefinition definition;
        while ((definition = @this?.Resolve()) != null)
        {
            if (definition.BaseType.IsSameAs(@base))
            {
                return true;
            }

            @this = definition.BaseType;
        }

        return false;
    }

    public static bool IsSameAs(this TypeReference @this, TypeReference other)
    {
        if (@this is null || other is null)
        {
            return false;
        }

        return @this.FullName == other.FullName && @this.GetAssemblyName() == other.GetAssemblyName();
    }

    public static string GetAssemblyName(this TypeReference @this)
    {
        return @this.Scope.MetadataScopeType switch
        {
            MetadataScopeType.AssemblyNameReference => ((AssemblyNameReference)@this.Scope).FullName,
            MetadataScopeType.ModuleReference => ((ModuleReference)@this.Scope).Name,
            _ => ((ModuleDefinition)@this.Scope).Assembly.FullName,
        };
    }
}
