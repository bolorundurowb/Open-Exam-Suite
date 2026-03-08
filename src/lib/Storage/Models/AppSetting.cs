/*
 *  Created by bolorundurowb on 2/1/2018
 */

namespace Storage.Models;

public class AppSetting
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;
}