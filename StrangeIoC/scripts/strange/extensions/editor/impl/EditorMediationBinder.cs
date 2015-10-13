/*
 * Copyright 2015 StrangeIoC
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *  http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

/**
 * @class strange.extensions.editor.impl.EditorMediationBinder
 * 
 * Binds EditorViews to EditorMediators.
 * 
 * Please read strange.extensions.mediation.api.IMediationBinder
 * where I've extensively explained the purpose of View mediation
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using strange.extensions.injector.api;
using strange.extensions.mediation.api;
using strange.extensions.mediation.impl;
using strange.framework.api;
using strange.framework.impl;
using UnityEngine;
using UnityEditor;

namespace strange.extensions.editor.impl
{
    public class EditorMediationBinder : AbstractMediationBinder
    {

        protected Dictionary<IView, List<IMediator>> mediators = new Dictionary<IView, List<IMediator>>();

        protected override IView[] GetViews(IView view)
        {
            EditorWindow editorWindow = view as EditorWindow;
            IView[] views = new IView[] { editorWindow as IView };
            return views;
        }

        protected override bool HasMediator(IView view, Type mediatorType)
        {
            IMediator mediator = FindMediator(view, mediatorType);
            return mediator != null;
        }

        /// Create a new Mediator object based on the mediatorType on the provided view
        protected override object CreateMediator(IView view, Type mediatorType)
        {
            IMediator mediator = Activator.CreateInstance(mediatorType) as IMediator;
            if (!mediators.ContainsKey(view))
            {
                mediators[view] = new List<IMediator>();

            }
            List<IMediator> editorMediators = mediators[view];

            Debug.Log("Mediator create: " + mediator);

            editorMediators.Add(mediator);
            return mediator;
        }

        /// Destroy the Mediator on the provided view object based on the mediatorType
        protected override object DestroyMediator(IView view, Type mediatorType)
        {
            IMediator mediator = FindMediator(view, mediatorType);
            if (mediator != null)
            {
                mediator.OnRemove();
            }
            return mediator;
        }

        protected override object EnableMediator(IView view, Type mediatorType)
        {
            IMediator mediator = FindMediator(view, mediatorType);
            if (mediator != null)
            {
                mediator.OnEnabled();
            }
            return mediator;
        }

        protected override object DisableMediator(IView view, Type mediatorType)
        {
            IMediator mediator = FindMediator(view, mediatorType);
            if (mediator != null)
            {
                mediator.OnDisabled();
            }
            return mediator;
        }

        protected override void ThrowNullMediatorError(Type viewType, Type mediatorType)
        {
            throw new MediationException("The view: " + viewType.ToString() + " is mapped to mediator: " + mediatorType.ToString() + ". AddComponent resulted in null, which probably means " + mediatorType.ToString().Substring(mediatorType.ToString().LastIndexOf(".") + 1) + " is not a MonoBehaviour.", MediationExceptionType.NULL_MEDIATOR);
        }

        protected IMediator FindMediator(IView view, Type mediatorType)
        {
            IMediator mediator = null;
            if (mediators.ContainsKey(view))
            {
                List<IMediator> editorMediators = mediators[view];
                foreach (var m in editorMediators)
                {
                    if (m.GetType() == mediatorType)
                    {
                        mediator = m;
                        break;
                    }
                }
            }
            return mediator;
        }
    }
}

