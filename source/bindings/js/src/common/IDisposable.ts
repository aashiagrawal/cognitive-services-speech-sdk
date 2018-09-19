//
// Copyright (c) Microsoft. All rights reserved.
// See https://aka.ms/csspeech/license201809 for the full license information.
//

/**
 *
 *
 * @export
 * @interface IDisposable
 */
export interface IDisposable {

    /**
     *
     *
     * @returns {boolean}
     *
     * @memberOf IDisposable
     */
    IsDisposed(): boolean;

    /**
     * Performs cleanup operations on this instance
     *
     * @param {string} [reason] optional reason for disposing the instance.
     * This will be used to throw errors when a operations are performed on the disposed object.
     *
     * @memberOf IDisposable
     */
    Dispose(reason?: string): void;
}
