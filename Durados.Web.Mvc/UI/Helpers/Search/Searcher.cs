﻿using Durados.DataAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Durados.Web.Mvc.UI.Helpers.Search
{
    public class Searcher
    {
        public object Search(string q, string entityType, int? id, int snippetLength)
        {
            q = q.Replace(" ", "%");

            if (string.IsNullOrEmpty(entityType))
            {
                return Search(q, (EntityType?)null, id, snippetLength);
            }

            EntityType entity;
            
            if (!Enum.TryParse<EntityType>(entityType, out entity))
            {
                throw new DuradosException("entityType is not valid");
            }

            return Search(q, entity, id, snippetLength);
        }

        ConfigAccess config = new ConfigAccess();

        public object Search(string q, EntityType? entityType, int? id, int snippetLength)
        {
            Dictionary<string, object> results = new Dictionary<string, object>();

            foreach (EntityType entityType2 in GetEntityTypes(entityType))
            {
                object entityResults = SearchEntity(q, entityType2, id, snippetLength);
                if (entityResults != null)
                {
                    results.Add(entityType2.ToString(), entityResults);
                }
            }

            return results;
        }

        private IEnumerable<EntityType> GetEntityTypes(EntityType? entityType)
        {
            if (entityType.HasValue)
            {
                return new EntityType[1] { entityType.Value };
            }
            else
            {
                return (IEnumerable<EntityType>)Enum.GetValues(typeof(EntityType));
            }

        }

        private object SearchEntity(string q, EntityType entityType, int? id, int snippetLength)
        {
            ConfigFieldSearcher[] configFieldSearchers = GetViewAndFieldName(entityType);

            Dictionary<string, object> results = new Dictionary<string, object>();

            foreach (ConfigFieldSearcher configFieldSearcher in configFieldSearchers)
            {
                object entityResults = configFieldSearcher.Search(config, q, id, snippetLength);
                if (entityResults != null)
                {
                    results.Add(configFieldSearcher.Name, entityResults);
                }
            }

            if (results.Count == 0)
            {
                return null;
            }
            return results;
        }

        private ConfigFieldSearcher[] GetViewAndFieldName(EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.Action:
                    return new ConfigFieldSearcher[2] { new ConfigFieldSearcher() { Name = "actionName", ViewName = "Rule", FieldName = "Name" }, new SnippetConfigFieldSearcher() { Name = "actionCode", ViewName = "Rule", FieldName = "Code" } };

                case EntityType.Object:
                    return new ConfigFieldSearcher[2] { new ConfigFieldSearcher() { Name = "objectName", ViewName = "View", FieldName = "Name" }, new FieldNameConfigFieldSearcher() { Name = "fieldName", ViewName = "Field", FieldName = "Name" } };

                case EntityType.Query:
                    return new ConfigFieldSearcher[2] { new ConfigFieldSearcher() { Name = "queryName", ViewName = "Query", FieldName = "Name" }, new SnippetConfigFieldSearcher() { Name = "querySql", ViewName = "Query", FieldName = "SQL" } };

                default:
                    return null;
            }
        }
    }
}
