// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Loading

open type Raylib_CSharp.Raylib

open O21.Game
open O21.Game.U95

type LoadingScene(window: WindowParameters, u95DataDirectory: string) =

    inherit LoadingSceneBase<U95Data>(window)

    override _.Load(lt, controller) = U95Data.Load lt controller u95DataDirectory
