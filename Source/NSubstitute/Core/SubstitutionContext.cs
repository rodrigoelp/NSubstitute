using System;
using System.Collections.Generic;
using NSubstitute.Exceptions;
using NSubstitute.Proxies;
using NSubstitute.Proxies.CastleDynamicProxy;
using NSubstitute.Proxies.DelegateProxy;
using NSubstitute.Routing.Definitions;

namespace NSubstitute.Core
{
    public class SubstitutionContext : ISubstitutionContext
    {
        public static ISubstitutionContext Current { get; set; }

        readonly ISubstituteFactory _substituteFactory;
        ICallRouter _lastCallRouter;
        IList<IArgumentSpecification> _argumentSpecifications;
        Func<ICall, object[]> _getArgumentsForRaisingEvent;

        static SubstitutionContext()
        {
            Current = new SubstitutionContext();
        }

        SubstitutionContext()
        {
            var callRouterFactory = new CallRouterFactory();
            var interceptorFactory = new CastleInterceptorFactory();
            var dynamicProxyFactory = new CastleDynamicProxyFactory(interceptorFactory);
            var delegateFactory = new DelegateProxyFactory();
            var proxyFactory = new ProxyFactory(delegateFactory, dynamicProxyFactory);
            var callRouteResolver = new CallRouterResolver();
            _substituteFactory = new SubstituteFactory(this, callRouterFactory, proxyFactory, callRouteResolver);
            _argumentSpecifications = new List<IArgumentSpecification>();
        }

        public SubstitutionContext(ISubstituteFactory substituteFactory)
        {
            _substituteFactory = substituteFactory;
        }

        public ISubstituteFactory SubstituteFactory { get { return _substituteFactory; } }

        public void LastCallShouldReturn(IReturn value, MatchArgs matchArgs)
        {            
            if (_lastCallRouter == null) throw new SubstituteException();
            _lastCallRouter.LastCallShouldReturn(value, matchArgs);
        }

        public void LastCallRouter(ICallRouter callRouter)
        {
            _lastCallRouter = callRouter;
            RaiseEventIfSet(callRouter);
        }

        void RaiseEventIfSet(ICallRouter callRouter)
        {
            if (_getArgumentsForRaisingEvent != null)
            {
                callRouter.SetRoute<RaiseEventRoute>(_getArgumentsForRaisingEvent);
                _getArgumentsForRaisingEvent = null;
            }
        }

        public ISubstituteFactory GetSubstituteFactory()
        {
            return SubstituteFactory;
        }

        public ICallRouter GetCallRouterFor(object substitute)
        {
            return SubstituteFactory.GetCallRouterCreatedFor(substitute);
        }

        public void EnqueueArgumentSpecification(IArgumentSpecification spec)
        {
            _argumentSpecifications.Add(spec);
        }

        public IList<IArgumentSpecification> DequeueAllArgumentSpecifications()
        {
            var result = _argumentSpecifications;
            _argumentSpecifications = new List<IArgumentSpecification>();
            return result;
        }

        public void RaiseEventForNextCall(Func<ICall, object[]> getArguments)
        {
            _getArgumentsForRaisingEvent = getArguments;
        }
    }
}