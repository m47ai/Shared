namespace M47.Shared.Tests.Infrastructure.Integration.Event;

using M47.Shared.Domain.Bus.Event;
using M47.Shared.Domain.Messages;
using M47.Shared.Infrastructure.Integration.Bus.Event;
using System;

public class DomainEventJsonSerializerTests : BaseTest
{
    [Fact]
    public void Should_FollowDefinedSchema_When_DomainEventAreSerialized()
    {
        // Arrange
        const string expected = @"{
                                    'data': {},
                                    'meta': {
                                        'messageId': 'a6cfe028-cdc1-41ff-bdab-a4db67917358',
                                        'type': 'DomainEventTest',
                                        'topic': {
                                            'name': 'organization.department.service.version.event.resource.eventName',
                                            'organization': 'organization',
                                            'department': 'department',
                                            'service': 'service',
                                            'version': 'version',
                                            'messageType': 'event',
                                            'resource': 'resource',
                                            'eventName': 'eventName'
                                        },
                                        'occurredOn': '2021-07-19T08:58:26.3444462Z'
                                    }
                                }";

        // Act
        var actual = DomainEventJsonSerializer.Serialize(CreateObject());

        // Assert
        ShouldJsonBeEquivalent(actual, expected);
    }

    private Message<DomainEventTest> CreateObject()
    {
        const string fullQualifiedEventName = "organization.department.service.version.event.resource.eventName";

        var eventId = Guid.Parse("a6cfe028-cdc1-41ff-bdab-a4db67917358");
        var ocurredOn = new DateTime(2021, 07, 19, 8, 58, 26, DateTimeKind.Utc).AddTicks(3444462);

        return new Message<DomainEventTest>(new DomainEventTest(eventId, ocurredOn, fullQualifiedEventName));
    }
}

public class DomainEventTest : DomainEvent
{
    private readonly string FullQualifiedEventName;

    public DomainEventTest(Guid eventId, DateTime ocurredOn, string fullQualifiedEventName)
        : base(eventId, ocurredOn)
    {
        FullQualifiedEventName = fullQualifiedEventName;
    }

    public override string GetFullQualifiedEventName() => FullQualifiedEventName;
}