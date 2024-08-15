// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Loading

open type Raylib_CsLo.Raylib

open O21.Game

type PreloadingScene(window: WindowParameters) =

    interface ILoadingScene<unit, LocalContent> with
        member this.Camera: Raylib_CsLo.Camera2D = Raylib_CsLo.Camera2D(zoom = 1f)

        member this.Init _ = ()
        member this.Load(lt, _) = LocalContent.Load(lt, window)

        member _.Update(_, _) = ()
        member _.Draw() = ClearBackground(BLACK)
