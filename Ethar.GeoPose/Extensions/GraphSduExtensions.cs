// <copyright file="GraphSduExtensions.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Ethar.GeoPose.Authority;
    using Ethar.GeoPose.StructuralDataUnits;

    /// <summary>
    /// Extensions for the <see cref="GraphSdu"/>.
    /// </summary>
    public static class GraphSduExtensions
    {
        /// <summary>
        /// Validates that the <see cref="GraphSdu"/> has does not have any transform indices out of bounds. Also validates that every node is
        /// extrinsic or reachable from an extrinsic node.
        /// </summary>
        /// <param name="sdu">The <see cref="GraphSdu"/> to validate.</param>
        /// <returns>True if the sdu is valid, false otherwise.</returns>
        public static bool Validate(this GraphSdu sdu)
        {
            var nodeCount = sdu.FrameList.Count;

            if (sdu.TransformList.Any(f => f.InnerFrameIndex >= nodeCount || f.OuterFrameIndex >= nodeCount))
            {
                return false;
            }

            var extrinsicOrReachableNodeIndices = sdu.FrameList.Where((f, i) =>
            {
                var authority = AuthorityProvider.GetAuthority(f.Authority);
                return authority.IsFrameSpecificationExtrinsic(f);
            }).Select((f, i) => i).ToList<int>();

            if (!extrinsicOrReachableNodeIndices.Any())
            {
                return false;
            }

            var graph = new Dictionary<int, IList<int>>();
            foreach (var pair in sdu.TransformList)
            {
                if (graph.ContainsKey(pair.OuterFrameIndex))
                {
                    graph[pair.OuterFrameIndex].Add(pair.InnerFrameIndex);
                }
                else
                {
                    graph.Add(pair.OuterFrameIndex, new List<int> { pair.InnerFrameIndex });
                }
            }

            foreach (var edge in graph)
            {
                var outerFrame = edge.Key;
                foreach (var innerFrame in edge.Value)
                {
                    VisitEdge(graph, extrinsicOrReachableNodeIndices, outerFrame, innerFrame);
                }
            }

            return extrinsicOrReachableNodeIndices.Count == nodeCount;
        }

        private static void VisitEdge(Dictionary<int, IList<int>> graph, List<int> reachableNodes, int outerFrame, int innerFrame)
        {
            if (reachableNodes.Contains(innerFrame) || !reachableNodes.Contains(outerFrame))
            {
                return;
            }

            if (reachableNodes.Contains(outerFrame))
            {
                reachableNodes.Add(innerFrame);
            }

            if (graph.TryGetValue(innerFrame, out var toVisit))
            {
                foreach (var index in toVisit)
                {
                    VisitEdge(graph, reachableNodes, innerFrame, index);
                }
            }
        }
    }
}
