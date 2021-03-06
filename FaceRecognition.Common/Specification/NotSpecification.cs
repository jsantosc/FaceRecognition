﻿using System;
using System.Linq;
using System.Linq.Expressions;
using Conditions;

namespace FaceRecognition.Common.Specification
{
    /// <summary>
    /// NotEspecification convert a original
    /// specification with NOT logic operator
    /// </summary>
    /// <typeparam name="TEntity">Type of element for this specificaiton</typeparam>
    public sealed class NotSpecification<TEntity>
        : Specification<TEntity>
        where TEntity : class
    {
        #region Members

        readonly Expression<Func<TEntity, bool>> _originalCriteria;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for NotSpecificaiton
        /// </summary>
        /// <param name="originalSpecification">Original specification</param>
        public NotSpecification(ISpecification<TEntity> originalSpecification)
        {
            Condition.Requires(originalSpecification, nameof(originalSpecification)).IsNotNull();

            _originalCriteria = originalSpecification.SatisfiedBy();
        }

        /// <summary>
        /// Constructor for NotSpecification
        /// </summary>
        /// <param name="originalSpecification">Original specificaiton</param>
        public NotSpecification(Expression<Func<TEntity, bool>> originalSpecification)
        {
            Condition.Requires(originalSpecification, nameof(originalSpecification)).IsNotNull();

            _originalCriteria = originalSpecification;
        }

        #endregion

        #region Override Specification methods

        /// <summary>
        /// <see cref="Specification{TEntity}"/>
        /// </summary>
        /// <returns><see cref="Specification{TEntity}"/></returns>
        public override Expression<Func<TEntity, bool>> SatisfiedBy()
        {

            return Expression.Lambda<Func<TEntity, bool>>(Expression.Not(_originalCriteria.Body),
                _originalCriteria.Parameters.Single());
        }

        #endregion
    }
}