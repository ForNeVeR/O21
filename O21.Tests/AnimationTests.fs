// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Tests.AnimationTests

open System.Threading.Tasks
open O21.Game.Animations
open Raylib_CSharp.Colors
open Raylib_CSharp.Images
open Raylib_CSharp.Textures
open Raylib_CSharp.Windowing
open Xunit

let testTextureLazy = lazy (
    Window.Init(1, 1, "Test Window")
    Texture2D.LoadFromImage(Image.GenColor(1, 1, Color.Red))
)

let testTexture = testTextureLazy.Value

let createAnimation length loopTime ticksPerFrame direction =
    Animation.Init(testTexture |> Array.create length, loopTime, ticksPerFrame, direction)
    
let playFullCycles animation n=
    let fullTicks =
        n * Checked.uint64 animation.Frames.Length * animation.TicksPerFrame
    let newTick = 
        snd animation.CurrentFrame + fullTicks
        - Checked.uint64 (fst animation.CurrentFrame) * animation.TicksPerFrame
    animation.Update newTick

[<Collection("AnimationTests")>]
type AnimationTestsFixture() =
    interface IAsyncLifetime with
        member _.InitializeAsync() =
            Task.CompletedTask
        member _.DisposeAsync() =
            if testTexture.IsValid() then
                testTexture.Unload()
            if Window.IsReady() then
                Window.Close()
            Task.CompletedTask

[<CollectionDefinition("AnimationTests")>]
type AnimationTestsCollection() =
    interface ICollectionFixture<AnimationTestsFixture>

[<Collection("AnimationTests")>]
[<Trait("Category", "ExcludeCI")>]
module BaseAnimationTests =
    
    [<Fact>]
    let ``Animation stops when TicksPerFrame is 0`` (): unit =
        let animation = createAnimation 1 (LoopTime.Count 1) 0UL AnimationDirection.Forward
        let updated = animation.Update(1UL)
        Assert.Equal(Some animation, updated)

    [<Theory>]
    [<InlineData(10, 1UL, AnimationDirection.Forward)>]
    [<InlineData(10, 1UL, AnimationDirection.Backward)>]
    let ``Animation plays a certain number of times``(frames, tpf, direction): unit =
        let cycles = 10
        let animation = createAnimation frames (LoopTime.Count cycles) tpf direction
        let animation = playFullCycles animation (Checked.uint64 cycles)
        Assert.True(animation.IsSome)
        let animation = playFullCycles animation.Value 1UL
        Assert.Equal(None, animation)
        
    [<Theory>]
    [<InlineData(10, 1UL, AnimationDirection.Forward)>]
    [<InlineData(10, 1UL, AnimationDirection.Backward)>]
    let ``Animation can plays a infinity time``(frames, tpf, direction): unit =
        let animation = createAnimation frames LoopTime.Infinity tpf direction
        
        let animation = animation.Update ((Checked.uint64 frames * tpf) - 1UL)
        Assert.True(animation.IsSome)
        
        // Animation should be on the last frame
        match direction with
        | AnimationDirection.Forward ->
            Assert.Equal(frames - 1, fst animation.Value.CurrentFrame)
        | AnimationDirection.Backward ->
            Assert.Equal(0, fst animation.Value.CurrentFrame)
        | _ ->
            Assert.Fail "Unexpected animation direction"
        
        let animation = playFullCycles animation.Value (Checked.uint64 1)
        Assert.True(animation.IsSome)
        
        // Full cycle should change the frame on start position
        match direction with
        | AnimationDirection.Forward ->
            Assert.Equal(0, fst animation.Value.CurrentFrame)
        | AnimationDirection.Backward ->
            Assert.Equal(frames - 1, fst animation.Value.CurrentFrame)
        | _ ->
            Assert.Fail "Unexpected animation direction"
