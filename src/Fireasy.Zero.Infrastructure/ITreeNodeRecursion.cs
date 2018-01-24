// -----------------------------------------------------------------------
// <copyright company="Fireasy"
//      email="faib920@126.com"
//      qq="55570729">
//   (c) Copyright Fireasy. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Fireasy.Common.ComponentModel;

namespace Fireasy.Zero.Infrastructure
{
    public interface ITreeNodeRecursion<T> : ITreeNode<T>
    {
        string Code { get; set; }

        int OrderNo { get; set; }
    }
}
