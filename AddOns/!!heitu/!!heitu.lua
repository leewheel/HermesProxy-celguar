-- zyf
-- function FixActionCount(self)
-- 	local text = self.Count
-- 	local action = self.action
-- 	local count = GetActionCount(action)
-- 	local _b = 0;

-- 	local actionType, id, subType = GetActionInfo(action)
-- 	if IsItemAction(action) and count == 0 then
-- 		count = GetItemCount(id)
-- 		_b = 1;
-- 	end

-- 	if (not IsItemAction(action) and count > 0) or _b==1 then
-- 		if count > (self.maxDisplayCount or 999) then
-- 			self.Count:SetText("*")
-- 		else
-- 			self.Count:SetText(count)
-- 		end
-- 	end
-- end


-- function ActionButton_UpdateUsable(self)
-- 	local icon = self.icon;
-- 	local normalTexture = self.NormalTexture;
-- 	if ( not normalTexture ) then
-- 		return;
-- 	end
-- 	-- 首先

-- 	local isUsable, notEnoughMana = IsUsableAction(self.action);
-- 	print("--------");
-- 	print(self.action);
-- 	print(isUsable);
-- 	print(notEnoughMana);


-- 	if ( isUsable ) then
-- 		icon:SetVertexColor(1.0, 1.0, 1.0);
-- 		normalTexture:SetVertexColor(1.0, 1.0, 1.0);
-- 	elseif ( notEnoughMana ) then
-- 		icon:SetVertexColor(1.0, 1.0, 1.0);
-- 		normalTexture:SetVertexColor(1.0, 1.0, 1.0);
-- 	else
-- 		icon:SetVertexColor(0.4, 0.4, 0.4);
-- 		normalTexture:SetVertexColor(1.0, 1.0, 1.0);
-- 	end
-- end

local GetActionCountSuper = GetActionCount
local IsUsableActionSuper = IsUsableAction

function GetActionCount(action)
	local count = GetActionCountSuper(action)
	local actionType, id, subType = GetActionInfo(action)
	if actionType == "macro" then
		local _, itemLink = GetMacroItem(id)
		local itemID = itemLink and GetItemInfoFromHyperlink(itemLink)
		if itemID and count==0 then
			count = GetItemCount(itemID)
		end
	end
	if IsItemAction(action) and count == 0 then
		count = GetItemCount(id)
	end

	return count;
end

function IsUsableAction(action)
	local isUsable, notEnoughMana = IsUsableActionSuper(action);
	local actionType, id, subType = GetActionInfo(action)
	local _b = 0;
	local count = GetActionCountSuper(action)

	if actionType == "macro" then
		local _, itemLink = GetMacroItem(id)
		local itemID = itemLink and GetItemInfoFromHyperlink(itemLink)
		if itemID and count==0 then
			count = GetItemCount(itemID)
			if count>0 then
				_b = 1;
			end
		end
	end

	if IsItemAction(action) and count == 0 then
		count = GetItemCount(id)
		if count>0 then
			_b = 1;
		end
	end
	if _b==1 then
		isUsable = true;
	end
	return isUsable, notEnoughMana
end

hooksecurefunc("GetActionCount", GetActionCount)
hooksecurefunc("IsUsableAction", IsUsableAction)

GetActionCount = GetActionCount
IsUsableAction = IsUsableAction

-- hooksecurefunc("ActionButton_UpdateCount", FixActionCount)
-- hooksecurefunc("ActionButton_UpdateUsable", ActionButton_UpdateUsable)

-- hooksecurefunc("ActionButton_UpdateUsable", ActionButton_UpdateUsable)

