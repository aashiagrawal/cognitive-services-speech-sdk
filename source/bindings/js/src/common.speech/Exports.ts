//
// Copyright (c) Microsoft. All rights reserved.
// See https://aka.ms/csspeech/license201809 for the full license information.
//

// IMPORTANT - Dont publish internal modules.

export * from "./CognitiveSubscriptionKeyAuthentication";
export * from "./CognitiveTokenAuthentication";
export * from "./IAuthentication";
export * from "./IConnectionFactory";
export * from "./IntentConnectionFactory";
export * from "./RecognitionEvents";
export * from "./Recognizer";
export * from "./RecognizerConfig";
export * from "./SpeechServiceInterfaces";
export * from "./WebsocketMessageFormatter";
export * from "./SpeechConnectionFactory";
export * from "./TranslationConnectionFactory";
export * from "./EnumTranslation";

export const OutputFormatPropertyName: string = "OutputFormat";
