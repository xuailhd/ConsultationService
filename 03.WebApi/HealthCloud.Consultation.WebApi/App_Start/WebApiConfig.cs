using HealthCloud.Consultation.WebApi.Filters;
using Swashbuckle.Application;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Description;
using HealthCloud.Consultation.Enums;
using HealthCloud.Common.Extensions;
using HealthCloud.Consultation.Common;
using System.ComponentModel;
using System.IO;
using HealthCloud.Common.Validations;
using HealthCloud.Consultation.Dto;

namespace HealthCloud.Consultation.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            ConfigSwagger(config);
            // Web API 配置和服务
            #region 配置路由
            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { controller = "Task", action = "Index", id = RouteParameter.Optional }
            );

            #endregion

            //异常处理
            config.Filters.Add(new ApiExceptionFilterAttribute());

            #region 设置api的返回结果类型

            config.Formatters.Remove(config.Formatters.XmlFormatter);

            //默认返回 json  
            config.Formatters
                .JsonFormatter.MediaTypeMappings.Add(
                new QueryStringMapping("datatype", "json", "application/json"));


            //json 序列化设置  
            config.Formatters
                .JsonFormatter.SerializerSettings = new Newtonsoft.Json.JsonSerializerSettings()
                {
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore, //设置忽略值为 null 的属性  
                    //DateTimeZoneHandling = DateTimeZoneHandling.Local,  //DateTime默认为本地时区
                    //DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffzzz"

                };


            #endregion

            
        }

        private static void ConfigSwagger(HttpConfiguration configuration)
        {
            //var apiExplorer = configuration.AddVersionedApiExplorer(o => o.GroupNameFormat = "'v'VVV");
            configuration
                    .EnableSwagger(c =>
                    {
                        // By default, the service root url is inferred from the request used to access the docs.
                        // However, there may be situations (e.g. proxy and load-balanced environments) where this does not
                        // resolve correctly. You can workaround this by providing your own code to determine the root URL.
                        //
                        //c.RootUrl(req => req.GetRequestContext().Url.Content("/api"));

                        // If schemes are not explicitly provided in a Swagger 2.0 document, then the scheme used to access
                        // the docs is taken as the default. If your API supports multiple schemes and you want to be explicit
                        // about them, you can use the "Schemes" option as shown below.
                        //
                        c.Schemes(new[] { "http", "https" });

                        // Use "SingleApiVersion" to describe a single version API. Swagger 2.0 includes an "Info" object to
                        // hold additional metadata for an API. Version and title are required but you can also provide
                        // additional fields by chaining methods off SingleApiVersion.
                        //
                        //c.SingleApiVersion("v1", "HealthCloud.Membership.Business.Web");

                        // If you want the output Swagger docs to be indented properly, enable the "PrettyPrint" option.
                        //
                        c.PrettyPrint();

                        // If your API has multiple versions, use "MultipleApiVersions" instead of "SingleApiVersion".
                        // In this case, you must provide a lambda that tells Swashbuckle which actions should be
                        // included in the docs for a given API version. Like "SingleApiVersion", each call to "Version"
                        // returns an "Info" builder so you can provide additional metadata per API version.
                        //
                        //c.MultipleApiVersions(
                        //    (apiDescription, version) => apiDescription.GetGroupName() == version,
                        //    (info) =>
                        //    {
                        //        foreach (var group in apiExplorer.ApiDescriptions)
                        //        {
                        //            var description = "API Doc";

                        //            if (@group.IsDeprecated)
                        //            {
                        //                description += " This API version has been deprecated.";
                        //            }

                        //            info.Version(@group.Name, $"API {@group.ApiVersion}")
                        //                    //.Contact(c1 => c1.Name("Bill Mei").Email("bill.mei@somewhere.com"))
                        //                    .Description(description);
                        //            //.License(l => l.Name("MIT").Url("https://opensource.org/licenses/MIT"))
                        //            //.TermsOfService("Shareware");
                        //        }
                        //    });
                        c.SingleApiVersion("v1","v1");

                        // You can use "BasicAuth", "ApiKey" or "OAuth2" options to describe security schemes for the API.
                        // See https://github.com/swagger-api/swagger-spec/blob/master/versions/2.0.md for more details.
                        // NOTE: These only define the schemes and need to be coupled with a corresponding "security" property
                        // at the document or operation level to indicate which schemes are required for an operation. To do this,
                        // you'll need to implement a custom IDocumentFilter and/or IOperationFilter to set these properties
                        // according to your specific authorization implementation
                        //
                        //c.BasicAuth("basic")
                        //    .Description("Basic HTTP Authentication");
                        //
                        // NOTE: You must also configure 'EnableApiKeySupport' below in the SwaggerUI section
                        //c.ApiKey("apiKey")
                        //    .Description("API Key Authentication")
                        //    .Name("apiKey")
                        //    .In("header");
                        //
                        //c.OAuth2("oauth2")
                        //    .Description("OAuth2 Implicit Grant")
                        //    .Flow("implicit")
                        //    .AuthorizationUrl("http://petstore.swagger.wordnik.com/api/oauth/dialog")
                        //    //.TokenUrl("https://tempuri.org/token")
                        //    .Scopes(scopes =>
                        //    {
                        //        scopes.Add("read", "Read access to protected resources");
                        //        scopes.Add("write", "Write access to protected resources");
                        //    });

                        // Set this flag to omit descriptions for any actions decorated with the Obsolete attribute
                        //c.IgnoreObsoleteActions();

                        // Each operation be assigned one or more tags which are then used by consumers for various reasons.
                        // For example, the swagger-ui groups operations according to the first tag of each operation.
                        // By default, this will be controller name but you can use the "GroupActionsBy" option to
                        // override with any value.
                        //
                        c.GroupActionsBy(apiDesc => apiDesc.ActionDescriptor.ControllerDescriptor.ControllerName);

                        // You can also specify a custom sort order for groups (as defined by "GroupActionsBy") to dictate
                        // the order in which operations are listed. For example, if the default grouping is in place
                        // (controller name) and you specify a descending alphabetic sort order, then actions from a
                        // ProductsController will be listed before those from a CustomersController. This is typically
                        // used to customize the order of groupings in the swagger-ui.
                        //
                        //c.OrderActionGroupsBy(new DeletegateCompare<string>((a, b) => string.Compare(a, b, StringComparison.Ordinal)));

                        // If you annotate Controllers and API Types with
                        // Xml comments (http://msdn.microsoft.com/en-us/library/b2s063f7(v=vs.110).aspx), you can incorporate
                        // those comments into the generated docs and UI. You can enable this by providing the path to one or
                        // more Xml comment files.
                        //
                        foreach (var s in ApplySchemaVendorExtensions.GetXmlCommentsPath())
                        {
                            c.IncludeXmlComments(s);
                        }

                        // Swashbuckle makes a best attempt at generating Swagger compliant JSON schemas for the various types
                        // exposed in your API. However, there may be occasions when more control of the output is needed.
                        // This is supported through the "MapType" and "SchemaFilter" options:
                        //
                        // Use the "MapType" option to override the Schema generation for a specific type.
                        // It should be noted that the resulting Schema will be placed "inline" for any applicable Operations.
                        // While Swagger 2.0 supports inline definitions for "all" Schema types, the swagger-ui tool does not.
                        // It expects "complex" Schemas to be defined separately and referenced. For this reason, you should only
                        // use the "MapType" option when the resulting Schema is a primitive or array type. If you need to alter a
                        // complex Schema, use a Schema filter.
                        //
                        //c.MapType<ProductType>(() => new Schema { type = "integer", format = "int32" });

                        // If you want to post-modify "complex" Schemas once they've been generated, across the board or for a
                        // specific type, you can wire up one or more Schema filters.
                        //
                        c.SchemaFilter<ApplySchemaVendorExtensions>();

                        // In a Swagger 2.0 document, complex types are typically declared globally and referenced by unique
                        // Schema Id. By default, Swashbuckle does NOT use the full type name in Schema Ids. In most cases, this
                        // works well because it prevents the "implementation detail" of type namespaces from leaking into your
                        // Swagger docs and UI. However, if you have multiple types in your API with the same class name, you'll
                        // need to opt out of this behavior to avoid Schema Id conflicts.
                        //
                        //c.UseFullTypeNameInSchemaIds();

                        // Alternatively, you can provide your own custom strategy for inferring SchemaId's for
                        // describing "complex" types in your API.
                        //
                        //c.SchemaId(t => t.FullName.Contains('`') ? t.FullName.Substring(0, t.FullName.IndexOf('`')) : t.FullName);

                        // Set this flag to omit schema property descriptions for any type properties decorated with the
                        // Obsolete attribute
                        //c.IgnoreObsoleteProperties();

                        // In accordance with the built in JsonSerializer, Swashbuckle will, by default, describe enums as integers.
                        // You can change the serializer behavior by configuring the StringToEnumConverter globally or for a given
                        // enum type. Swashbuckle will honor this change out-of-the-box. However, if you use a different
                        // approach to serialize enums as strings, you can also force Swashbuckle to describe them as strings.
                        //
                        //c.DescribeAllEnumsAsStrings();

                        // Similar to Schema filters, Swashbuckle also supports Operation and Document filters:
                        //
                        // Post-modify Operation descriptions once they've been generated by wiring up one or more
                        // Operation filters.
                        //
                        c.OperationFilter<ApplySchemaVendorExtensions>();
                        //
                        // If you've defined an OAuth2 flow as described above, you could use a custom filter
                        // to inspect some attribute on each action and infer which (if any) OAuth2 scopes are required
                        // to execute the operation
                        //
                        //c.OperationFilter<AssignOAuth2SecurityRequirements>();

                        // Post-modify the entire Swagger document by wiring up one or more Document filters.
                        // This gives full control to modify the final SwaggerDocument. You should have a good understanding of
                        // the Swagger 2.0 spec. - https://github.com/swagger-api/swagger-spec/blob/master/versions/2.0.md
                        // before using this option.
                        //
                        c.DocumentFilter<ApplySchemaVendorExtensions>();
#pragma warning disable CS0618 // 类型或成员已过时
                        c.ApplyFiltersToAllSchemas();
#pragma warning restore CS0618 // 类型或成员已过时
                        // In contrast to WebApi, Swagger 2.0 does not include the query string component when mapping a URL
                        // to an action. As a result, Swashbuckle will raise an exception if it encounters multiple actions
                        // with the same path (sans query string) and HTTP method. You can workaround this by providing a
                        // custom strategy to pick a winner or merge the descriptions for the purposes of the Swagger docs
                        //
                        //c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                        // Wrap the default SwaggerGenerator with additional behavior (e.g. caching) or provide an
                        // alternative implementation for ISwaggerProvider with the CustomProvider option.
                        //
                        //c.CustomProvider((defaultProvider) => new CachingSwaggerProvider(defaultProvider));
                    })
                    .EnableSwaggerUi(c =>
                    {
                        // Use the "DocumentTitle" option to change the Document title.
                        // Very helpful when you have multiple Swagger pages open, to tell them apart.
                        //
                        //c.DocumentTitle("My Swagger UI");

                        // Use the "InjectStylesheet" option to enrich the UI with one or more additional CSS stylesheets.
                        // The file must be included in your project as an "Embedded Resource", and then the resource's
                        // "Logical Name" is passed to the method as shown below.
                        //
                        //c.InjectStylesheet(containingAssembly, "Swashbuckle.Dummy.SwaggerExtensions.testStyles1.css");

                        // Use the "InjectJavaScript" option to invoke one or more custom JavaScripts after the swagger-ui
                        // has loaded. The file must be included in your project as an "Embedded Resource", and then the resource's
                        // "Logical Name" is passed to the method as shown above.
                        //
                        //c.InjectJavaScript(thisAssembly, "Swashbuckle.Dummy.SwaggerExtensions.testScript1.js");

                        // The swagger-ui renders boolean data types as a dropdown. By default, it provides "true" and "false"
                        // strings as the possible choices. You can use this option to change these to something else,
                        // for example 0 and 1.
                        //
                        //c.BooleanValues(new[] { "0", "1" });

                        // By default, swagger-ui will validate specs against swagger.io's online validator and display the result
                        // in a badge at the bottom of the page. Use these options to set a different validator URL or to disable the
                        // feature entirely.
                        //c.SetValidatorUrl("http://localhost/validator");
                        //c.DisableValidator();

                        // Use this option to control how the Operation listing is displayed.
                        // It can be set to "None" (default), "List" (shows operations for each resource),
                        // or "Full" (fully expanded: shows operations and their details).
                        //
                        //c.DocExpansion(DocExpansion.List);

                        // Specify which HTTP operations will have the 'Try it out!' option. An empty paramter list disables
                        // it for all operations.
                        //
                        //c.SupportedSubmitMethods("GET", "HEAD");

                        // Use the CustomAsset option to provide your own version of assets used in the swagger-ui.
                        // It's typically used to instruct Swashbuckle to return your version instead of the default
                        // when a request is made for "index.html". As with all custom content, the file must be included
                        // in your project as an "Embedded Resource", and then the resource's "Logical Name" is passed to
                        // the method as shown below.
                        //
                        //c.CustomAsset("index", containingAssembly, "YourWebApiProject.SwaggerExtensions.index.html");

                        // If your API has multiple versions and you've applied the MultipleApiVersions setting
                        // as described above, you can also enable a select box in the swagger-ui, that displays
                        // a discovery URL for each version. This provides a convenient way for users to browse documentation
                        // for different API versions.
                        //
                        //c.EnableDiscoveryUrlSelector();

                        // If your API supports the OAuth2 Implicit flow, and you've described it correctly, according to
                        // the Swagger 2.0 specification, you can enable UI support as shown below.
                        //
                        //c.EnableOAuth2Support(
                        //    clientId: "test-client-id",
                        //    clientSecret: null,
                        //    realm: "test-realm",
                        //    appName: "Swagger UI"
                        //    //additionalQueryStringParams: new Dictionary<string, string>() { { "foo", "bar" } }
                        //);

                        // If your API supports ApiKey, you can override the default values.
                        // "apiKeyIn" can either be "query" or "header"
                        //
                        //c.EnableApiKeySupport("apiKey", "header");
                        c.DisableValidator();
                    });
        }
    }

    internal class ApplySchemaVendorExtensions : ISchemaFilter, IDocumentFilter, IOperationFilter
    {
        public static string[] GetXmlCommentsPath()
        {
            return Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "/bin", "HealthCloud*.xml");
        }

        private static MethodInfo _genericMethodDefinition;

        /// <summary>初始化 <see cref="T:System.Object" /> 类的新实例。</summary>
        static ApplySchemaVendorExtensions()
        {
            Func<((string Name, string Desc, int value)[] Enums, string Desc)> c = GetDocForEnum<ConsoleKey>;
            _genericMethodDefinition = c.Method.GetGenericMethodDefinition();
        }

        void ISchemaFilter.Apply(Schema schema, SchemaRegistry schemaRegistry, Type type)
        {
            schema.vendorExtensions.Add("typeInfo", type.FriendlyId());

            foreach(var par in type.GetProperties())
            {
                if (IsIgnoreAttribute(par))
                {
                    schema.properties.Remove(par.Name);
                }
            }

            if (type.IsEnum)
            {
                SetupEnumType(schema, type);
            }
            if (IsCloseGenericeType(type, typeof(IEnumerable<>)))
            {
                var t = type.GetGenericArguments()[0];
                if (t.IsEnum)
                {
                    SetupEnumType(schema, t, false, false);
                }
            }
            if (!type.IsEnum && type != typeof(string))
            {
                SetupDataForEnum(schema, type);
            }
        }

        private static bool IsCloseGenericeType(Type t, Type baseType)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == baseType)
            {
                return true;
            }
            if (t.IsGenericType && baseType.IsInterface && t.GetInterfaces().Any(x => IsCloseGenericeType(x, baseType)))
            {
                return true;
            }
            return false;
        }

        private static Type GetUnderlyingTypeForNullable(Type t)
        {
            var type = typeof(Nullable<>);
            return t.IsValueType && IsCloseGenericeType(t, type) ? Nullable.GetUnderlyingType(t) : t;
        }

        private static void SetupDataForEnum(Schema schema, Type type)
        {
            if (schema.properties == null)
            {
                return;
            }
            foreach (var propertyInfo in type.GetProperties())
            {
                var pType = GetUnderlyingTypeForNullable(propertyInfo.PropertyType);

                foreach (var pair in schema.properties)
                {
                    if (pair.Key.Equals(propertyInfo.Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        var schemaProp = pair.Value;
                        var requireIfAttribute = propertyInfo.GetCustomAttribute<RequireIfAttribute>();
                        if (requireIfAttribute != null)
                        {
                            schemaProp.description += $"({requireIfAttribute.GetDescription()})";
                        }
                        if (pType.IsEnum)
                        {
                            var ret = GetEnumDesc("", pType);
                            if (ret.ok)
                            {
                                schemaProp.description += ret.desc;
                            }
                        }
                        break;
                    }
                }
            }
        }

        private static void SetupEnumType(Schema schema, Type type, bool setDescription = false, bool setEnum = true)
        {
            var ret = GetEnumDesc(schema.description, type);
            if (ret.ok)
            {
                if (setEnum)
                {
                    schema.@enum = ret.enums;
                }
                if (setDescription)
                {
                    schema.description += ret.desc;
                }
                schema.vendorExtensions.Add("enumValues", ret.enums);
                schema.vendorExtensions.Add("enumDesc", ret.desc);
                //schema.type = type.Name;
            }
        }

        void IDocumentFilter.Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            RemoveVersionInPaths(swaggerDoc);
        }

        private static void RemoveVersionInPaths(SwaggerDocument swaggerDoc)
        {
            //swaggerDoc.paths = new Dictionary<string, PathItem>(swaggerDoc.paths.ToDictionary(x => { return x.Key.Replace("v{apiVersion}", swaggerDoc.info.version); }, x =>
            //{
            //    x.Value.parameters?.Remove(x.Value.parameters?.First(k => k.name == "apiVersion"));
            //    x.Value.get?.parameters?.Remove(x.Value.get?.parameters?.First(k => k.name == "apiVersion"));
            //    x.Value.post?.parameters?.Remove(x.Value.post?.parameters?.First(k => k.name == "apiVersion"));
            //    x.Value.delete?.parameters?.Remove(x.Value.delete?.parameters?.First(k => k.name == "apiVersion"));
            //    x.Value.put?.parameters?.Remove(x.Value.put?.parameters?.First(k => k.name == "apiVersion"));
            //    x.Value.patch?.parameters?.Remove(x.Value.patch?.parameters?.First(k => k.name == "apiVersion"));
            //    x.Value.head?.parameters?.Remove(x.Value.head?.parameters?.First(k => k.name == "apiVersion"));
            //    x.Value.options?.parameters?.Remove(x.Value.options?.parameters?.First(k => k.name == "apiVersion"));
            //    return x.Value;
            //}));
        }

        //private void Rev(IDictionary<string, Schema> definitions)
        //{
        //    foreach (var definition in definitions)
        //    {
        //        var schema = definition.Value;
        //        if (schema.properties != null)
        //        {
        //            Rev(schema.properties);
        //            continue;
        //        }
        //        if (schema.@enum != null)
        //        {
        //            var type = schema.@enum.FirstOrDefault().GetType();
        //            var v = GetEnumDesc(schema.description, type);
        //            if (v.ok)
        //            {
        //                schema.@enum = v.enums;
        //                schema.description = v.desc;
        //            }
        //        }
        //    }
        //}

        private static (bool ok, string desc, IList<object> enums) GetEnumDesc(string desc, Type type)
        {
            if (type == null || !type.IsEnum)
            {
                return (false, null, null);
            }
            var method = _genericMethodDefinition.MakeGenericMethod(type);
            var e = (((string Name, string Desc, int value)[] Enums, string Desc))method.Invoke(null, EmptyArray<object>.Value);
            var enums = e.Enums;
            var types = type.GetCustomAttributes<EnumExtendAttribute>().Select(x => x.ExtendTypes).Aggregate(new List<Type>(), (a, b) =>
            {
                a.AddRange(b);
                return a;
            });
            foreach (var type1 in types)
            {
                var tuple = (((string Name, string Desc, int value)[] Enums, string Desc))_genericMethodDefinition.MakeGenericMethod(type1).Invoke(null, EmptyArray<object>.Value);
                enums = tuple.Enums.Where(x => enums.All(k => k.value != x.value)).Concat(enums).ToArray();
            }
            var values = enums.OrderBy(x => x.value).ToArray();
            var description = $" [枚举值] {{{string.Join(", ", values.Select(x => $"{x.Desc.IfNull(() => x.Name)} = {x.value}"))}}}";
            //schema.description = description;
            var @enum = values.Select(x => (object)x.value).ToList();
            //schema.@enum = @enum;
            return (true, description, @enum);
        }

        public static ((string Name, string Desc, int value)[] Enums, string Desc) GetDocForEnum<T>() where T : struct
        {
            var enums = Enum.GetValues(typeof(T)).Cast<T>().Select(x => (x.ToString(), x.GetDescription(), x.ToInt())).ToArray();
            return (enums, typeof(T).GetTypeInfo().GetCustomAttribute<DescriptionAttribute>()?.Description);
        }

        /// <summary>
        /// Applies the filter to the specified operation using the given context.
        /// </summary>
        /// <param name="operation">The operation to apply the filter to.</param>
        /// <param name="schemaRegistry">The API schema registry.</param>
        /// <param name="apiDescription">The API description being filtered.</param>
        void IOperationFilter.Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            ProcessParameter(operation, schemaRegistry, apiDescription);
            ProcessReturn(operation, schemaRegistry, apiDescription);
        }

        private void ProcessReturn(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var description = apiDescription.ActionDescriptor.GetCustomAttributes<ReturnModelDescriptionAttribute>();
            if (description != null && description.Count > 0)
            {
                foreach (var keyValuePair in operation.responses)
                {
                    var s = $"{keyValuePair.Value.schema.@ref}_{operation.operationId}";
                    var key = s.Substring("#/definitions/".Length);
                    if (!schemaRegistry.Definitions.ContainsKey(key))
                    {
                        var schema = schemaRegistry.Definitions[keyValuePair.Value.schema.@ref.Substring("#/definitions/".Length)];
                        var schemaCopy = schema.Map<Schema, Schema>();
                        schemaCopy.properties = new Dictionary<string, Schema>(schemaCopy.properties);
                        foreach (var g in description.GroupBy(x => x.Property ?? "").Where(x => !x.Key.IsNullOrWhiteSpace()))
                        {
                            var prop = g.Key;
                            var schemaCopyProperty = schemaCopy.properties[prop] = schemaCopy.properties[prop].Map<Schema, Schema>();
                            var xs = string.Join(", ", g.Select(x => x.Description).Distinct());
                            schemaCopyProperty.description = (s.IsNullOrWhiteSpace() ? "" : xs + " ") + schemaCopyProperty.description;
                            if (schemaCopyProperty.vendorExtensions.ContainsKey("enumDesc"))
                            {
                                schemaCopyProperty.description += schemaCopyProperty.vendorExtensions["enumDesc"];
                            }
                        }
                        {
                            var xs = string.Join(", ", description.Where(x => x.Property.IsNullOrWhiteSpace()).Select(x => x.Description).Distinct());
                            schemaCopy.description = (s.IsNullOrWhiteSpace() ? "" : xs + " ") + schemaCopy.description;
                        }
                        schemaRegistry.Definitions.Add(key, schemaCopy);
                    }
                    keyValuePair.Value.schema.@ref = s;
                }
                //pair.Value.description += " [Data]: " + string.Join(", ", description.Select(x => x.Description));
            }
        }

        private static void ProcessParameter(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (operation.parameters == null)
            {
                return;
            }

            foreach (var parameter in operation.parameters)
            {
                var description = apiDescription.ParameterDescriptions.FirstOrDefault(p => p.Name == parameter.name);
                if (description == null)
                {
                    continue;
                }

                // REF: https://github.com/domaindrivendev/Swashbuckle/issues/1101
                if (parameter.description == null)
                {
                    parameter.description = description.Documentation;
                }

                // REF: https://github.com/domaindrivendev/Swashbuckle/issues/1089
                // REF: https://github.com/domaindrivendev/Swashbuckle/pull/1090
                if (parameter.@default == null)
                {
                    parameter.@default = description.ParameterDescriptor.DefaultValue;
                }
            }

            foreach (var parameter in apiDescription.ActionDescriptor.GetParameters())
            {
            } 
        }

        private static bool IsIgnoreAttribute(PropertyInfo property)
        {
            var customAttributes = property.GetCustomAttributes(typeof(IgnoreInDocAttribute), true);
            if (customAttributes.Length > 0)
            {
                return true;
            }
            return false;
        }
    }
}
