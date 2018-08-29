$(document).ready(function ()
{
	var AllEntities = $.ajax({ url: "/Home/GetAllEntityName", async: false });
	var jsonObj = eval('(' + AllEntities.responseText + ')');
	var entities = jsonObj.Entities;
	var entityArray = [];
	var fieldArray = [];
	var returnSearch;

	var AllTables = $.ajax({ url: "/Home/GetAllTableName", async: false });
	var jsonTable = eval('(' + AllTables.responseText + ')');
	var tables = jsonTable.TableList;
	var tableArray = [];

	for (i = 0; i < tables.length; i++)
	{
		var entity = tables[i];
		tableArray.push(entity.Name)
	}

	for (i = 0; i < entities.length; i++)
	{
		var entity = entities[i];
		entityArray.push(entity.Name)
		var fields = entity.Fields;

		for (y = 0 ; y < fields.length; y++)
		{
			fieldArray.push(fields[y].FieldProperties[0].PropertyValue);
		}
	}

	var substringMatcher = function (strs)
	{
		return function findMatches(q, cb)
		{
			var matches, substrRegex;
			matches = [];
			substrRegex = new RegExp(q, 'i');

			$.each(strs, function (i, str)
			{

				if (substrRegex.test(str))
				{
					matches.push({ value: str });
				}

			});
			cb(matches);
		};
	};

	$('#the-basics .typeahead').typeahead({
		hint: true,
		highlight: true,
		minLength: 1
	},
{
	name: 'Table',
	display: 'value',
	source: substringMatcher(tableArray)
});

	$('#multiple-datasets .typeahead').typeahead({
		highlight: true
	},
	{
		name: 'Entity',
		display: 'value',
		source: substringMatcher(entityArray),
		templates: {
			header: '<h3 class="league-name">Entity</h3>'
		}
	},
	{
		name: 'Fields',
		display: 'value',
		source: substringMatcher(fieldArray),
		templates: {
			header: '<h3 class="league-name">Field</h3>'
		}
	});

	$("#Search").keyup(function (event)
	{
		if (event.keyCode === 13)
		{
			var entityName = $('#Search')[0].value;
			GetEntityByName(entityName);
		}
	});

	$("#SearchTable").keyup(function (event)
	{
		if (event.keyCode === 13)
		{
			var entityName = $('#SearchTable')[0].value;
			GetTableInfo(entityName);
		}
	});

	$("#primarySearchButton").click(function ()
	{
		var entityName = $('#Search')[0].value;
		GetEntityByName(entityName);
	})


	function GetEntityByName(name)
	{
		$.ajax({
			url: "/Home/SearchResult",
			data: { name: name },
			async: false,
			success: function (data)
			{
				$("#TableContent").html(data);
				hljs.configure({ useBR: false });
				$('pre code').each(function (i, block)
				{
					hljs.highlightBlock(block);
				});
				hightLightKeyWords(name);
			}
		});
	};

	function hightLightKeyWords(keyWord)
	{
		$('.hljs-string').each(function ()
		{
			if ($(this)[0].innerText == '"' + keyWord + '"')
			{
				$(this).addClass("keyWord");
			}
		})
	};

	function GetTableInfo(tableName)
	{
		$.ajax({
			url: "/Home/TableSearchResult",
			data: { tableName: tableName },
			async: false,
			success: function (data)
			{
				$("#TableContent").html(data);
				hightLightKeyWords(tableName);
				//hljs.configure({ useBR: false });
				//$('pre code').each(function (i, block)
				//{
				//	hljs.highlightBlock(block);
				//});
				//hightLightKeyWords(name);
			}
		});
	}
});
