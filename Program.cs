using System;
using System.Collections.Generic;

namespace Design_Patterns
{
    public enum Color
    {
        Red,
        Green,
        Blue
    }

    public enum Size
    {
        Small, Medium, Large, Huge
    }

    public class Product
    {
        public string Name;
        public Color Color;
        public Size Size;

        public Product(string name, Color color, Size size)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Color = color;
            Size = size;
        }
    }

    public class ProductFilter
    {
        #region Incorrect way -> Violating open-closed principle
        public IEnumerable<Product> FilterBySize(IEnumerable<Product> products, Size size)
        {
            foreach (var p in products)
            {
                if (p.Size == size)
                {
                    yield return p;
                }
            }
        }

        public IEnumerable<Product> FilterByColor(IEnumerable<Product> products, Color color)
        {
            foreach (var p in products)
            {
                if (p.Color == color)
                {
                    yield return p;
                }
            }
        }

        public IEnumerable<Product> FilterBySizeAndColor(IEnumerable<Product> products, Size size, Color color)
        {
            foreach (var p in products)
            {
                if (p.Size == size && p.Color == color)
                {
                    yield return p;
                }
            }
        }
        #endregion
    }

    public interface ISpecification<T>
    {
        bool IsSatisfied(T t);
    }

    public interface IFilter<T>
    {
        IEnumerable<T> Filter(IEnumerable<T> items, ISpecification<T> spec);
    }

    public class ColorSpecification : ISpecification<Product>
    {
        private Color color;

        public ColorSpecification(Color color)
        {
            this.color = color;
        }

        public bool IsSatisfied(Product t)
        {
            return t.Color == color;
        }
    }

    public class SizeSpecification : ISpecification<Product>
    {
        private Size size;

        public SizeSpecification(Size size)
        {
            this.size = size;
        }

        public bool IsSatisfied(Product t)
        {
            return t.Size == size;
        }
    }

    public class AndSpecification<T> : ISpecification<T>
    {
        //ISpecification<T> first, second;
        ISpecification<T>[] specs;

        //public AndSpecification(ISpecification<T> first, ISpecification<T> second)
        //{
        //    this.first = first ?? throw new ArgumentNullException(nameof(first));
        //    this.second = second ?? throw new ArgumentNullException(nameof(second));
        //}

        public AndSpecification(ISpecification<T>[] specs)
        {
            this.specs = specs;
        }

        public bool IsSatisfied(T t)
        {
            bool satisfied = true;
            foreach (var spec in specs)
            {
                satisfied = satisfied && spec.IsSatisfied(t);
                if (!satisfied)
                {
                    break;
                }
            }
            return satisfied;
        }

        //public bool IsSatisfied(T t)
        //{
        //    return first.IsSatisfied(t) && second.IsSatisfied(t);
        //}
    }

    public class BetterFilter : IFilter<Product>
    {
        public IEnumerable<Product> Filter(IEnumerable<Product> items, ISpecification<Product> spec)
        {
            foreach (var i in items)
            {
                if (spec.IsSatisfied(i))
                {
                    yield return i;
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var apple = new Product("Apple", Color.Green, Size.Small);
            var tree = new Product("Tree", Color.Green, Size.Large);
            var house = new Product("House", Color.Blue, Size.Large);

            Product[] products = { apple, tree, house };

            var pf = new ProductFilter();
            Console.WriteLine("Green products (old):");

            foreach (var p in pf.FilterByColor(products, Color.Green))
            {
                Console.WriteLine($" - {p.Name} is green");
            }

            var bf = new BetterFilter();
            Console.WriteLine("Green Products (new):");
            foreach (var p in bf.Filter(products, new ColorSpecification(Color.Green)))
            {
                Console.WriteLine($" - {p.Name} is green");
            }

            Console.WriteLine("Large Products (new):");
            foreach (var p in bf.Filter(products, new SizeSpecification(Size.Large)))
            {
                Console.WriteLine($" - {p.Name} is huge");
            }

            //Console.WriteLine("Large Blue Products (new):");
            //foreach (var p in bf.Filter(products, new AndSpecification<Product>(new ColorSpecification(Color.Blue), new SizeSpecification(Size.Large))))
            //{
            //    Console.WriteLine($" - {p.Name} is Large and Blue");
            //}

            Console.WriteLine("Large and Blue products with AndSpecification array");
            ISpecification<Product>[] specifications = { new ColorSpecification(Color.Blue), new SizeSpecification(Size.Large) };
            foreach (var p in bf.Filter(products, new AndSpecification<Product>(specifications)))
            {
                Console.WriteLine($" - {p.Name} is Large and Blue");
            }

        }
    }
}
