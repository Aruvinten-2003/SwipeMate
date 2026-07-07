using Microsoft.AspNetCore.Authentication;
using SwipeMate.Application.Features.Auth.Commands.Login;
using SwipeMate.Application.Features.Auth.Commands.ForgotPassword;
using SwipeMate.Application.Features.Auth.Commands.RefreshToken;
using SwipeMate.Application.Features.Auth.Commands.Register;
using SwipeMate.Application.Features.Discovery.Queries.GetDiscoveryFeed;
using SwipeMate.Application.Features.Matches.Commands.Unmatch;
using SwipeMate.Application.Features.Matches.Queries.GetMatches;
using SwipeMate.Application.Features.Messaging.Commands.SendMessage;
using SwipeMate.Application.Features.Messaging.Queries.GetConversations;
using SwipeMate.Application.Features.Messaging.Queries.GetMessages;
using SwipeMate.Application.Features.Moderation.Commands.ResolveReport;
using SwipeMate.Application.Features.Moderation.Queries.GetReports;
using SwipeMate.Application.Features.Profiles.Commands.DeletePhoto;
using SwipeMate.Application.Features.Profiles.Commands.UpdateProfile;
using SwipeMate.Application.Features.Profiles.Commands.UploadPhoto;
using SwipeMate.Application.Features.Profiles.Queries.GetMyProfile;
using SwipeMate.Api.Authorization;
using SwipeMate.Api.Hubs;
using SwipeMate.Application.Features.Safety.Commands.BlockUser;
using SwipeMate.Application.Features.Safety.Commands.ReportUser;
using SwipeMate.Application.Features.Swipes.Commands.RecordSwipe;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services
    .AddAuthentication(DemoAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, DemoAuthenticationHandler>(
        DemoAuthenticationHandler.SchemeName,
        options => { });
builder.Services.AddAuthorization(options => options.AddSwipeMatePolicies());
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
        policy.WithOrigins(
                "http://localhost:5173",
                "http://127.0.0.1:5173",
                "http://localhost:5174",
                "http://127.0.0.1:5174")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});
builder.Services.AddScoped<RegisterCommandHandler>();
builder.Services.AddScoped<LoginCommandHandler>();
builder.Services.AddScoped<RefreshTokenCommandHandler>();
builder.Services.AddScoped<ForgotPasswordCommandHandler>();
builder.Services.AddScoped<GetDiscoveryFeedQueryHandler>();
builder.Services.AddScoped<GetMatchesQueryHandler>();
builder.Services.AddScoped<UnmatchCommandHandler>();
builder.Services.AddScoped<GetConversationsQueryHandler>();
builder.Services.AddScoped<GetMessagesQueryHandler>();
builder.Services.AddScoped<SendMessageCommandHandler>();
builder.Services.AddScoped<GetReportsQueryHandler>();
builder.Services.AddScoped<ResolveReportCommandHandler>();
builder.Services.AddScoped<GetMyProfileQueryHandler>();
builder.Services.AddScoped<UpdateProfileCommandHandler>();
builder.Services.AddScoped<UploadPhotoCommandHandler>();
builder.Services.AddScoped<DeletePhotoCommandHandler>();
builder.Services.AddScoped<RecordSwipeCommandHandler>();
builder.Services.AddScoped<BlockUserCommandHandler>();
builder.Services.AddScoped<ReportUserCommandHandler>();

var app = builder.Build();

app.UseCors("DefaultPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.Run();
