using OtekBillingMetering.Business.Abstractions;
using OtekBillingMetering.Business.Common.Exceptions;
using OtekBillingMetering.Business.Models.MenuItemModels.Types;

namespace OtekBillingMetering.Business.Models.MenuItemModels;

public sealed class MenuItem : Entity<Guid>
{
	private readonly List<MenuItem> _children = [];

	private MenuItem() : base() { }

	public MenuItem(
		string key,
		MenuItemAreaType area,
		MenuItemType type,
		string titleKey,
		string? titleFallback = null,
		string? route = null,
		string? externalUrl = null,
		string? icon = null,
		int sortOrder = 0,
		Guid? parentId = null)
	{
		UpdateKey(key);
		SetArea(area);
		SetType(type);
		UpdateTitle(titleKey, titleFallback);
		UpdateNavigation(route, externalUrl);
		UpdateIcon(icon);
		UpdateSortOrder(sortOrder);

		ParentId = parentId;

		IsVisible = true;
		IsEnabled = true;
	}

	public string Key { get; private set; } = null!;

	public MenuItemAreaType Area { get; private set; }
	public MenuItemType Type { get; private set; }

	public string TitleKey { get; private set; } = null!;
	public string? TitleFallback { get; private set; }

	public string? Route { get; private set; }
	public string? ExternalUrl { get; private set; }

	public string? Icon { get; private set; }

	public int SortOrder { get; private set; }

	public bool IsVisible { get; private set; }
	public bool IsEnabled { get; private set; }

	public Guid? ParentId { get; private set; }

	public MenuItem? Parent { get; private set; }
	public IReadOnlyCollection<MenuItem> Children => _children;

	public void UpdateKey(string key) => Key = string.IsNullOrWhiteSpace(key)
		? throw new DomainValidationException("Key is required.")
		: key.Trim();

	public void SetArea(MenuItemAreaType area) => Area = area;

	public void SetType(MenuItemType type) => Type = type;

	public void UpdateTitle(string titleKey, string? titleFallback)
	{
		TitleKey = string.IsNullOrWhiteSpace(titleKey)
			? throw new DomainValidationException("TitleKey is required.")
			: titleKey.Trim();

		TitleFallback = string.IsNullOrWhiteSpace(titleFallback) ? null : titleFallback.Trim();
	}

	public void UpdateNavigation(string? route, string? externalUrl)
	{
		Route = string.IsNullOrWhiteSpace(route) ? null : route.Trim();
		ExternalUrl = string.IsNullOrWhiteSpace(externalUrl) ? null : externalUrl.Trim();

		if(Type == MenuItemType.Item && Route is null)
		{
			throw new DomainValidationException("Route is required for Item.");
		}

		if(Type == MenuItemType.ExternalLink && ExternalUrl is null)
		{
			throw new DomainValidationException("ExternalUrl is required for ExternalLink.");
		}

		if(Type is MenuItemType.Group or MenuItemType.Divider)
		{
			Route = null;
			ExternalUrl = null;
		}
	}

	public void UpdateIcon(string? icon) =>
		Icon = string.IsNullOrWhiteSpace(icon) ? null : icon.Trim();

	public void UpdateSortOrder(int sortOrder) => SortOrder = sortOrder;

	public void SetVisible(bool value) => IsVisible = value;
	public void SetEnabled(bool value) => IsEnabled = value;

	public void SetParent(Guid? parentId) => ParentId = parentId;
}
