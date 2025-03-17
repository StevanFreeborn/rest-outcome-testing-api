using System.ComponentModel;

namespace API.Common;

record SuccessResponse(
  [property: Description("The message of the response")]
  string Message
);