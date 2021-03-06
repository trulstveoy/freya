﻿//----------------------------------------------------------------------------
//
// Copyright (c) 2014
//
//    Ryan Riley (@panesofglass) and Andrew Cherry (@kolektiv)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//----------------------------------------------------------------------------

[<AutoOpen>]
module Freya.Todo.Backend.Api

open System
open Freya.Core
open Freya.Core.Operators
open Freya.Inspector
open Freya.Machine
open Freya.Machine.Inspector
open Freya.Machine.Router
open Freya.Pipeline
open Freya.Pipeline.Operators
open Freya.Router
open Freya.Router.Inspector
open Freya.Types.Http

// Route

let id =
    memoM ((Option.get >> Guid.Parse) <!> getPLM (Route.valuesKey "id"))

// Body

let newTodo =
    memoM (body ())

let patchTodo =
    memoM (body ())

// Storage

let add =
    memoM (Freya.asyncM Storage.add =<< (Option.get <!> newTodo))

let clear =
    memoM (Freya.asyncM Storage.clear =<< Freya.returnM ())

let delete =
    memoM (Freya.asyncM Storage.delete =<< id)

let get =
    memoM (Freya.asyncM Storage.get =<< id)

let list =
    memoM (Freya.asyncM Storage.list =<< Freya.returnM ())

let update =
    memoM (Freya.asyncM Storage.update =<< (tuple <!> id <*> (Option.get <!> patchTodo)))

// Machine

let addAction =
    ignore <!> add

let addedHandler _ =
    represent <!> add

let clearAction =
    ignore <!> clear

let deleteAction =
    ignore <!> delete

let getHandler _ =
    represent <!> get

let listHandler _ =
    represent <!> list

let updateAction =
    ignore <!> update

// Resources

let todosMethods =
    Freya.returnM [ 
        DELETE
        GET
        OPTIONS
        POST ]

let todos =
    freyaMachine {
        including defaults
        corsMethodsSupported todosMethods
        methodsSupported todosMethods
        doDelete clearAction
        doPost addAction
        handleCreated addedHandler
        handleOk listHandler } |> compileFreyaMachine

let todoMethods =
    Freya.returnM [
        DELETE
        GET
        OPTIONS
        PATCH ]

let todo =
    freyaMachine {
        including defaults
        corsMethodsSupported todoMethods
        methodsSupported todoMethods
        doDelete deleteAction
        doPatch updateAction
        handleOk getHandler } |> compileFreyaMachine

// Routes

let todoBackend : FreyaPipeline =
    freyaRouter {
        resource "/" todos
        resource "/:id" todo } |> compileFreyaRouter

// API

let config =
    { Inspectors = 
        [ freyaRequestInspector
          freyaMachineInspector
          freyaRouterInspector ] }

let api =
    freyaInspector config >?= todoBackend
