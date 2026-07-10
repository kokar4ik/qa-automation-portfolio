namespace UnionReportingApi.Constants;

public static class ApiConstants
{
    public static class Endpoints
    {
        public const string TokenGet = "token/get";

        public const string TestPut = "test/put";

        public const string TestPutLog = "test/put/log";

        public const string TestPutAttachment = "test/put/attachment";

        public const string TestGetJson = "test/get/json";
    }

    public static class QueryParameters
    {
        public const string Variant = "variant";

        public const string ProjectId = "projectId";

        public const string TestId = "testId";
    }

    public static class FormParameters
    {
        public const string SessionId = "SID";

        public const string ProjectName = "projectName";

        public const string TestName = "testName";

        public const string MethodName = "methodName";

        public const string Environment = "env";

        public const string Browser = "browser";

        public const string TestId = "testId";

        public const string Content = "content";

        public const string IsException = "isException";
    }

    public static class Protocol
    {
        public const char UrlTrailingSlash = '/';

        public const char JsonArrayOpen = '[';

        public const char JsonArrayClose = ']';

        public const string BooleanTrue = "true";

        public const string BooleanFalse = "false";

        public const string MediaTypeImagePng = "image/png";

        public const int ErrorResponseSnippetMaxLength = 200;
    }

    public static class ResponseFormats
    {
        public const string Json = "JSON";
    }
}
