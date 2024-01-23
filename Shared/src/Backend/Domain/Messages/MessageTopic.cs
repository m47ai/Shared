namespace M47.Shared.Domain.Messages;

using System;
using System.Collections.Generic;
using System.Linq;

public class MessageTopic
{
    private const int _minElements = 7;
    private const int _maxElements = 8;

    private const int _organizationIndex = 0;
    private const int _departmentIndex = 1;
    private const int _serviceIndex = 2;
    private const int _messageVersionIndex = 3;
    private const int _messageTypeIndex = 4;
    private const int _resourceIndex = 5;
    private const int _subResourceIndex = 6;

    private readonly List<string> _messageTypeValues = new() { "event", "command" };

    public string Name { get; set; }
    public string Organization { get; set; }
    public string Department { get; set; }
    public string Service { get; set; }
    public string Version { get; set; }
    public string MessageType { get; set; }
    public string Resource { get; set; }
    public string? SubResource { get; set; }
    public string EventName { get; set; }

    public MessageTopic(string fullQualifiedName)
    {
        var topicParts = fullQualifiedName.Split(".");

        EnsureMaxParts(topicParts);
        EnsureMinPartsCount(topicParts);
        EnsureMessageType(topicParts);

        Name = fullQualifiedName;
        Organization = topicParts[_organizationIndex];
        Department = topicParts[_departmentIndex];
        Service = topicParts[_serviceIndex];
        Version = topicParts[_messageVersionIndex];
        MessageType = topicParts[_messageTypeIndex];
        Resource = topicParts[_resourceIndex];
        SubResource = topicParts.Length == _maxElements ? topicParts[_subResourceIndex] : null;
        EventName = topicParts.Last();
    }

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

    public MessageTopic()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    { }

    private void EnsureMessageType(string[] topicParts)
    {
        if (!_messageTypeValues.Contains(topicParts[_messageTypeIndex]))
        {
            throw new ArgumentException("Invalid topic parts in the domain event");
        }
    }

    private static void EnsureMinPartsCount(string[] topicParts)
    {
        if (topicParts.Length < _minElements)
        {
            throw new ArgumentException("poc");
        }
    }

    private static void EnsureMaxParts(string[] topicParts)
    {
        if (topicParts.Length > _maxElements)
        {
            throw new ArgumentException("massa");
        }
    }
}