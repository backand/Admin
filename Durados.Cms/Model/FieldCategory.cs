using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados.Cms.Model
{
    public partial class FieldCategory
    {
        private List<Field> fields = null;

        public bool IsEmpty(IEnumerable enumerable, DisplayFieldRules displayFieldRules, bool registered)
        {
            if (!FieldCategoryRel.IsLoaded)
                FieldCategoryRel.Load();

            foreach (FieldCategoryRel fieldCategory in FieldCategoryRel)
            {
                if (!fieldCategory.FieldReference.IsLoaded)
                    fieldCategory.FieldReference.Load();

                if (registered || !fieldCategory.Field.OnlyForRegistered)
                {
                    bool allFieldsHasValue = true;
                    foreach (object o in enumerable)
                    {
                        switch (displayFieldRules)
                        {
                            case DisplayFieldRules.Always:
                                return false;
                            case DisplayFieldRules.AtLeastOne:
                                if (fieldCategory.Field.GetValue(o) != null)
                                    return false;
                                break;
                            case DisplayFieldRules.MustAll:
                                if (fieldCategory.Field.GetValue(o) == null)
                                    allFieldsHasValue = false;
                                break;

                            default:
                                break;
                        }
                    }

                    if (displayFieldRules == DisplayFieldRules.MustAll && allFieldsHasValue)
                        return false;
                }
            }

            return true;
        }

        public bool IsEmpty(object o)
        {
            if (!FieldCategoryRel.IsLoaded)
                FieldCategoryRel.Load();

            foreach (FieldCategoryRel fieldCategory in FieldCategoryRel)
            {
                if (!fieldCategory.FieldReference.IsLoaded)
                    fieldCategory.FieldReference.Load();

                if (fieldCategory.Field.GetValue(o) != null)
                    return false;
            }

            return true;
        }

        public List<Field> Fields
        {
            get
            {
                if (fields == null)
                    LoadFields();

                return fields;
            }
        }

        private void LoadFields()
        {
            fields = new List<Field>();

            if (!FieldCategoryRel.IsLoaded)
                FieldCategoryRel.Load();

            foreach (FieldCategoryRel fieldCategory in FieldCategoryRel)
            {
                if (!fieldCategory.FieldReference.IsLoaded)
                    fieldCategory.FieldReference.Load();

                if (!fields.Contains(fieldCategory.Field))
                    fields.Add(fieldCategory.Field);
            }
        }
    }

    public enum DisplayFieldRules
    {
        Always = 0,
        AtLeastOne = 1,
        MustAll = 2,
    }
}
