// Copyright 2021 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Cloud.PubSub.V1;
using System.Collections.Generic;
using Xunit;

[Collection(nameof(PubsubFixture))]
public class PublishProtoMessagesAsyncTest
{
    private readonly PubsubFixture _pubsubFixture;
    private readonly PublishProtoMessagesAsyncSample _publishProtoMessagesAsyncSample;
    private readonly PullMessagesAsyncSample _pullMessagesAsyncSample;

    public PublishProtoMessagesAsyncTest(PubsubFixture pubsubFixture)
    {
        _pubsubFixture = pubsubFixture;
        _publishProtoMessagesAsyncSample = new PublishProtoMessagesAsyncSample();
        _pullMessagesAsyncSample = new PullMessagesAsyncSample();
    }

    [Fact]
    public async void PublishBinaryMessages()
    {
        string randomName = _pubsubFixture.RandomName();
        string topicId = $"testTopicForProtoBinaryMessageCreation{randomName}";
        string subscriptionId = $"testSubscriptionForProtoBinaryMessageCreation{randomName}";
        string schemaId = $"testSchemaForProtoBinaryMessageCreation{randomName}";

        var schema = _pubsubFixture.CreateProtoSchema(schemaId);
        _pubsubFixture.CreateTopicWithSchema(topicId, schema.Name.ToString(), Encoding.Binary);
        _pubsubFixture.CreateSubscription(topicId, subscriptionId);

        List<Utilities.State> messageTexts = new List<Utilities.State> { new Utilities.State { Name = "New York", PostAbbr = "NY" }, new Utilities.State { Name = "Pennsylvania", PostAbbr = "PA" } };

        var output = await _publishProtoMessagesAsyncSample.PublishProtoMessagesAsync(_pubsubFixture.ProjectId, topicId, messageTexts);
        Assert.Equal(messageTexts.Count, output);

        // Pull the Message to confirm it is valid
        await _pubsubFixture.Pull.Eventually(async () =>
        {
            var result = await _pullMessagesAsyncSample.PullMessagesAsync(_pubsubFixture.ProjectId, subscriptionId, false);
            Assert.True(result > 0);
        });
    }

    [Fact]
    public async void PublishJsonMessages()
    {
        string randomName = _pubsubFixture.RandomName();
        string topicId = $"testTopicForProtoJsonMessageCreation{randomName}";
        string subscriptionId = $"testSubscriptionForProtoJsonMessageCreation{randomName}";
        string schemaId = $"testSchemaForProtoJsonMessageCreation{randomName}";

        var schema = _pubsubFixture.CreateProtoSchema(schemaId);
        _pubsubFixture.CreateTopicWithSchema(topicId, schema.Name.ToString(), Encoding.Json);
        _pubsubFixture.CreateSubscription(topicId, subscriptionId);

        List<Utilities.State> messageTexts = new List<Utilities.State> { new Utilities.State { Name = "New York", PostAbbr = "NY" }, new Utilities.State { Name = "Pennsylvania", PostAbbr = "PA" } };

        var output = await _publishProtoMessagesAsyncSample.PublishProtoMessagesAsync(_pubsubFixture.ProjectId, topicId, messageTexts);
        Assert.Equal(messageTexts.Count, output);

        // Pull the Message to confirm it is valid
        await _pubsubFixture.Pull.Eventually(async () =>
        {
            var result = await _pullMessagesAsyncSample.PullMessagesAsync(_pubsubFixture.ProjectId, subscriptionId, false);
            Assert.True(result > 0);
        });
    }
}
